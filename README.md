# 🌐 JJodel Backend API

The application is publicly accessible via the frontend at:  
👉 https://app.jjodel.io

---

# ⚙️ Installation Guide - JJodel Backend

This guide provides complete instructions for installing and running the **JJodel .NET API** in different environments.

---

## 📋 Prerequisites

### For local development:
- .NET 10 SDK  
- PostgreSQL  
- Git  

### For Docker deployment:
- Docker & Docker Compose  
- Git  

---

## 🚀 Quick Installation with Docker

### Option 1: Pre-built image

```bash
docker run -p 8080:80 --name jjodel-backend \
  your-dockerhub/jjodel-backend:latest
