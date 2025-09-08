# Configuration Examples

This document provides comprehensive configuration examples for TallyMCP in various scenarios.

## Basic Configurations

### Development Environment
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "TallyMCP": {
    "Server": {
      "Host": "localhost",
      "Port": 3001
    },
    "Tally": {
      "Host": "localhost",
      "Port": 9000
    }
  }
}
```

### Production Environment
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "TallyMCP": {
    "Server": {
      "Host": "0.0.0.0",
      "Port": 3001
    },
    "Tally": {
      "Host": "192.168.1.100",
      "Port": 9000
    }
  }
}
```

## Environment Variable Examples

### Linux/macOS (.env file)
```bash
# Server Configuration
TallyMCP__Server__Host=0.0.0.0
TallyMCP__Server__Port=3001

# Tally Configuration
TallyMCP__Tally__Host=192.168.1.100
TallyMCP__Tally__Port=9000

# Logging Configuration
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft__AspNetCore=Warning
```

### Windows (batch file)
```batch
@echo off
set TallyMCP__Server__Host=0.0.0.0
set TallyMCP__Server__Port=3001
set TallyMCP__Tally__Host=192.168.1.100
set TallyMCP__Tally__Port=9000
TallyMCP.exe
```

### Docker Compose
```yaml
version: '3.8'
services:
  tallymcp:
    image: tallymcp:latest
    ports:
      - "3001:3001"
    environment:
      - TallyMCP__Server__Host=0.0.0.0
      - TallyMCP__Server__Port=3001
      - TallyMCP__Tally__Host=tally-server
      - TallyMCP__Tally__Port=9000
      - Logging__LogLevel__Default=Information
    restart: unless-stopped
```

## Command Line Examples

### Basic Startup
```bash
# Default configuration
./TallyMCP

# Specify custom server port
./TallyMCP --TallyMCP:Server:Port=8080

# Specify custom Tally server
./TallyMCP --TallyMCP:Tally:Host=192.168.1.100 --TallyMCP:Tally:Port=9000

# Enable debug logging
./TallyMCP --Logging:LogLevel:Default=Debug
```

### Advanced Configurations
```bash
# Complete custom configuration
./TallyMCP \
  --TallyMCP:Server:Host=0.0.0.0 \
  --TallyMCP:Server:Port=8080 \
  --TallyMCP:Tally:Host=192.168.1.100 \
  --TallyMCP:Tally:Port=9001 \
  --Logging:LogLevel:Default=Information

# Load custom config file
./TallyMCP --config=/path/to/custom-config.json
```

## Cloud Deployment Examples

### Azure Container Instances
```json
{
  "location": "East US",
  "name": "tallymcp-aci",
  "properties": {
    "containers": [
      {
        "name": "tallymcp",
        "properties": {
          "image": "tallymcp:latest",
          "ports": [
            {
              "port": 3001,
              "protocol": "TCP"
            }
          ],
          "environmentVariables": [
            {
              "name": "TallyMCP__Server__Host",
              "value": "0.0.0.0"
            },
            {
              "name": "TallyMCP__Server__Port",
              "value": "3001"
            }
          ],
          "resources": {
            "requests": {
              "cpu": 0.5,
              "memoryInGB": 1
            }
          }
        }
      }
    ],
    "osType": "Linux",
    "ipAddress": {
      "type": "Public",
      "ports": [
        {
          "port": 3001,
          "protocol": "TCP"
        }
      ]
    }
  }
}
```

### Kubernetes Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: tallymcp
spec:
  replicas: 1
  selector:
    matchLabels:
      app: tallymcp
  template:
    metadata:
      labels:
        app: tallymcp
    spec:
      containers:
      - name: tallymcp
        image: tallymcp:latest
        ports:
        - containerPort: 3001
        env:
        - name: TallyMCP__Server__Host
          value: "0.0.0.0"
        - name: TallyMCP__Server__Port
          value: "3001"
        - name: TallyMCP__Tally__Host
          value: "tally-service"
        - name: TallyMCP__Tally__Port
          value: "9000"
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
---
apiVersion: v1
kind: Service
metadata:
  name: tallymcp-service
spec:
  selector:
    app: tallymcp
  ports:
  - port: 3001
    targetPort: 3001
  type: LoadBalancer
```

## Security Configurations

### Reverse Proxy with Nginx
```nginx
server {
    listen 80;
    server_name tallymcp.example.com;
    
    location / {
        proxy_pass http://localhost:3001;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### systemd Service (Linux)
```ini
[Unit]
Description=TallyMCP Server
After=network.target

[Service]
Type=simple
User=tallymcp
WorkingDirectory=/opt/tallymcp
ExecStart=/opt/tallymcp/TallyMCP
Restart=always
RestartSec=5
Environment=TallyMCP__Server__Host=127.0.0.1
Environment=TallyMCP__Server__Port=3001
Environment=TallyMCP__Tally__Host=localhost
Environment=TallyMCP__Tally__Port=9000

[Install]
WantedBy=multi-user.target
```

### Windows Service
```xml
<configuration>
  <system.serviceModel>
    <services>
      <service name="TallyMCP">
        <endpoint address="http://localhost:3001"
                  binding="basicHttpBinding"
                  contract="ITallyMCP" />
      </service>
    </services>
  </system.serviceModel>
</configuration>
```