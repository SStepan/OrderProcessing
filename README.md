Orders Service — README

A small .NET 8 microservice that accepts orders via HTTP, enqueues them, and processes them asynchronously, persisting results to PostgreSQL.

⸻

How to run

1) Run everything with Docker

cd docker
docker compose up --build

Services:
•	API: http://localhost:8080
•	RabbitMQ UI: http://localhost:15672 (guest/guest)
•	Postgres: localhost:5432 (db: orders, user: postgres, pass: postgres)

EF Core applies migrations automatically on API/Worker startup and seeds a tiny inventory.

2) (Alternative) Run infra in Docker, debug API/Worker in Rider

cd docker && docker compose up -d postgres rabbitmq

Then run Orders.Api and Orders.Worker from Rider.
Use these dev settings:

ConnectionStrings:Default = Host=localhost;Username=postgres;Password=postgres;Database=orders
Rabbit:Host = localhost


⸻

Endpoints

Submit order

curl -X POST http://localhost:8080/orders \
-H "Content-Type: application/json" \
-d '{"customerId": 123, "items":[{"productId":1,"qty":2},{"productId":2,"qty":4}]}'
# 202 Accepted {"id":"<GUID>"}

Get order status

curl http://localhost:8080/orders/<GUID>
# { "orderId": "...", "status": "processed" | "failed", ... }

(Optional helper) Quick list of 10 last orders

curl http://localhost:8080/orders

⸻

Initial seed data
new Inventory { ProductId = 1, Sku = "SKU-1", UnitPriceCents = 1999, Qty = 10 },
new Inventory { ProductId = 2, Sku = "SKU-2", UnitPriceCents = 499,  Qty = 50 }

⸻

Design decisions & trade-offs

Messaging: RabbitMQ
•	Durable queues, acks, retries, DLQ capability; easy local Docker run.
•	API publishes an OrderSubmitted message; Worker consumes and processes.

Two processes (API & Worker)
•	Clear separation between request handling and async processing.
•	Demonstrates at-least-once messaging + idempotency.

Application layer used for business logic
•	API and Worker are thin shells; orchestration (validate, reserve stock, compute totals/discounts, mark status) lives in Orders.Application.
•	Easier to unit test and swap adapters.

Race-free stock allocation
•	Each order is claimed atomically (pending → processing) to avoid double work.
•	Guarded UPDATE per item inside a single DB transaction:

UPDATE inventory SET qty = qty - @need
WHERE product_id=@pid AND qty >= @need;

If any row update affects 0 rows → rollback and mark order failed: OUT_OF_STOCK.

Observability (basic only)
•	No Prometheus/Grafana.
•	Worker logs order_processed ... processed_count=N.

Not included (known gaps)
•	No Outbox pattern (there’s a tiny “saved but not published” window).
•	No retries/DLQ UI (RabbitMQ supports them; left out to stay minimal).
•	Happy-path validation only (e.g., prices are read from inventory).

⸻

Assumptions
•	Single currency; TotalCents recomputed from authoritative inventory prices.
•	Discount rule is illustrative: 10% off if total quantity ≥ 5, capped at $50.
•	No partial fulfillment; any stock shortage fails the whole order.
•	Inventory is a single row per product (unique product_id/sku).
•	Message delivery is at-least-once; consumer is idempotent via order status CAS.
