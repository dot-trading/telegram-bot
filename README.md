# 🤖 Telegram Bot (dot-trading)

![.NET](https://img.shields.io/badge/.NET-10.0-512bd4?style=for-the-badge&logo=dotnet)
![Telegram](https://img.shields.io/badge/Telegram-Bot-blue?style=for-the-badge&logo=telegram)
![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)

The **Telegram Bot** is the interactive interface of the `dot-trading` platform. It provides real-time notifications, market insights, and portfolio management directly on your phone. It acts as the primary bridge between the automated trading engine and the user.

---

## ✨ Key Features

*   **🔔 Real-time Alerts**: Instant notifications for trade opens, closes, and take-profit updates via MQTT.
*   **📊 Portfolio Management**:
    *   `/pnl`: Detailed P&L breakdown (Daily, Weekly, Monthly, Total).
    *   `/report`: Comprehensive summary of current holdings and performance.
    *   `/positions`: List of currently active trades with live unrealized P&L.
*   **🧠 Market Insights**:
    *   `/stress`: Check the Global Market Stress Score and risk scaling status.
    *   `/sentiment`: Get the current Fear & Greed Index.
*   **🛠 Admin Controls**: Manage bot settings and manual intervention if needed.

---

## 🏗 Architecture

The bot is built using a decoupled architecture, listening to both direct commands and asynchronous events:

```text
[ Orchestrator ] ──▶ [ NanoMQ Broker ] ──▶ [ TELEGRAM BOT ] ──▶ [ User Device ]
                                                │
[ Persistence ] <──▶ [ Persistence API ] <──────┘
```

---

## 🚀 Getting Started

### 1. Prerequisites
*   .NET 10 SDK
*   A Telegram Bot Token (from [@BotFather](https://t.me/botfather))
*   Access to the `dot-trading` MQTT broker

### 2. Configuration
Configure the bot via `appsettings.json` or Environment Variables:

| Variable | Description |
| :--- | :--- |
| `Telegram__Token` | Your unique Telegram Bot Token. |
| `Telegram__ChatId` | Your private Chat ID for restricted access. |
| `Mqtt__Host` | Hostname of the NanoMQ broker. |
| `Persistence__Url` | Endpoint for the Persistence API. |

---

## 🤝 Contributing & Support

If this project helps you in your trading journey, feel free to contribute or give it a ⭐!

### 💎 Donations
Community support helps keep the development active and covers API subscription costs.

*   **BTC**: `1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa`
*   **ETH (ERC20)**: `0x742d35Cc6634C0532925a3b844Bc454e4438f44e`
*   **USDT (TRC20)**: `TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t`

---

## 📄 License
Part of the [dot-trading](https://github.com/dot-trading) open-source trading platform. Licensed under MIT.
