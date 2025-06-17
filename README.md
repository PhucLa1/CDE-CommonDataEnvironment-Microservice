
# 🏗️ Common Data Environment (CDE) - Microservices Architecture

A modular C# microservices-based system for managing a **Common Data Environment** (CDE) in the context of digital construction, engineering collaboration, or smart document workflows.

---

## 🚀 Tech Stack

| Component       | Technology                     |
|----------------|---------------------------------|
| Language        | C# (.NET 8)                    |
| Communication   | gRPC, RabbitMQ                 |
| API Gateway     | YARP (Yet Another Reverse Proxy) |
| Services        | AI, Auth, Event, Project       |
| Architecture    | Microservices + Event-driven   |
| Transport       | HTTP/2, AMQP                   |
| Containerization| Docker, Docker Compose         |

---

## 🧩 Microservices Overview

| Service                 | Description                                                                 |
|-------------------------|-----------------------------------------------------------------------------|
| 🧠 `AI Service`     | Handles AI/ML logic such as document classification, metadata extraction. |
| 🔐 `Auth Service`   | Manages user registration, login, role-based access control using JWT.     |
| 📩 `Event Service`  | Processes domain events (e.g., file upload, project updates) and dispatches notifications using RabbitMQ. |
| 📁 `Project Service`| Manages CDE project data, files, folders, metadata, and related workflows. |

---

## 📡 Communication Architecture

```
Client → YARP API Gateway → gRPC → Services  
                      ↳ RabbitMQ → Event-Driven Communication
```

---

## 📁 Folder Structure

```
/src
  /Services
    /AIService
    /AuthService
    /EventService
    /ProjectService
  /Gateways
    /YarpApiGateway
  /Shared
    /Protos
    /Common
/docker-compose.yml
/README.md
```

---

## ⚙️ Getting Started

### 1. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker & Docker Compose](https://www.docker.com/)
- [RabbitMQ](https://www.rabbitmq.com/)
- [gRPCurl](https://github.com/fullstorydev/grpcurl) (Optional for gRPC testing)

---

### 2. Clone the Repository

```bash
git clone https://github.com/your-org/cde-microservices.git
cd cde-microservices
```

---

### 3. Run All Services

```bash
docker-compose up --build
```

By default:
- gRPC services exposed at `localhost:50XX`
- YARP API Gateway: `http://localhost:8000`
- RabbitMQ UI: `http://localhost:15672` (`guest`/`guest`)

---

## 🧪 Example: Testing with gRPCurl

```bash
grpcurl -plaintext localhost:5001 list
```

Or test a specific service method (e.g., list projects):

```bash
grpcurl -d '{}' localhost:5001 project.ProjectService/ListProjects
```

---

## 📬 Messaging: RabbitMQ Integration

This system uses RabbitMQ as a **message broker** for handling cross-service communication and domain events.

You can access the RabbitMQ dashboard at:

👉 [http://localhost:15672](http://localhost:15672)  
(Username: `guest`, Password: `guest`)

---

## 📦 .env and Configuration

Each service can have its own configuration via `appsettings.json` or environment variables.

Example of setting environment variables in `docker-compose.yml`:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - RabbitMQ__Host=rabbitmq
```

---

## 🧰 Tools and Libraries Used

- **YARP** - Reverse Proxy Gateway for routing HTTP/gRPC to services
- **gRPC** - High-performance communication between microservices
- **RabbitMQ** - Event-driven message queue
- **AutoMapper / FluentValidation / MediatR** - Clean architecture support (optional)
- **Docker Compose** - Multi-service orchestration

---

## 🚧 Roadmap

- [x] Microservice scaffolding
- [x] YARP gateway configuration
- [x] gRPC protocol communication
- [x] Basic Auth (JWT)
- [x] RabbitMQ integration
- [ ] AI module for document classification
- [ ] Full CI/CD with GitHub Actions
- [ ] Service Discovery & Monitoring (e.g., Prometheus + Grafana)

---

## 🤝 Contribution

Contributions are welcome! Please fork the repo and open a pull request.

---

## 📄 License

MIT License © 2025 Lã Hồng Phúc / UTC
