# Money Mirror 💰

A mobile financial literacy application for children and parents. Parents monitor spending, set allowances, and create savings goals. Children interact through a gamified interface with expense logging, quizzes, savings goals, and themed space characters.

---

## Team Structure

| Layer | Tech |
|---|---|
| Backend API | ASP.NET Core 8 Web API |
| Database | SQL Server |
| AI Service | Python Flask |

---

## Repository Structure

```
money-mirror/
├── backend/
│   └── MoneyMirror/
│       ├── MoneyMirror.API/          # Controllers, validators, filters, Program.cs
│       ├── MoneyMirror.Core/         # DTOs, models, interfaces, enums, helpers
│       └── MoneyMirror.Infrastructure/ # Services, DbContext, EF migrations, hubs
├── ai_service/
│   ├── api.py                        # Flask app entry point
│   ├── run.py                        # Dev server runner
│   ├── chatbot/                      # Chatbot logic, prompt builders, providers
│   ├── personality/                  # Scoring, profiling, weekly update
│   ├── quizzes/                      # Quiz generation scripts
│   └── tests/                        # Unit tests
└── README.md
```

---

## Prerequisites

### Backend
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote)
- Visual Studio 2022 or VS Code with C# extension

### AI Service
- Python 3.10+
- pip

---

## Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/your-org/money-mirror.git
cd money-mirror
```

---

### 2. Backend Setup

#### Configure `appsettings.json`

Edit `backend/MoneyMirror/MoneyMirror.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=MoneyMirror;Integrated Security=True;Trust Server Certificate=True"
  },
  "Jwt": {
    "SecretKey": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "MoneyMirrorAPI",
    "Audience": "MoneyMirrorApp",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "AIService": {
    "Url": "http://localhost:5000"
  },
  "SendGrid": {
    "ApiKey": "YOUR_SENDGRID_API_KEY",
    "SenderEmail": "noreply@yourdomain.com",
    "SenderName": "Money Mirror Team"
  },
  "QuizImport": {
    "JsonFilePath": "path/to/your/quiz.json",
    "ImportKey": "YOUR_IMPORT_KEY"
  }
}
```

#### Run migrations and start the API

```bash
cd backend/MoneyMirror
dotnet restore
dotnet ef database update --project MoneyMirror.Infrastructure --startup-project MoneyMirror.API
dotnet run --project MoneyMirror.API
```

API runs at:
- HTTP: `http://localhost:5054`
- HTTPS: `https://localhost:7048`
- Swagger: `https://localhost:7048/swagger`
- Hangfire dashboard: `https://localhost:7048/hangfire`

---

### 3. AI Service Setup

```bash
cd ai_service
pip install -r requirements.txt
```

Create a `.env` file in `ai_service/`:

```env
GEMINI_API_KEY=your_gemini_key
GROQ_API_KEY=your_groq_key
OPENROUTER_API_KEY=your_openrouter_key
```

Start the Flask server:

```bash
python run.py
```

AI service runs at `http://localhost:5000`.

> The backend expects the AI service on port 5000. Both must be running for personality calculation, weekly updates, and chatbot to work.

---

## API Overview

All responses follow this envelope:

```json
{
  "success": true,
  "message": "Human-readable message",
  "data": {},
  "errors": null
}
```

### Auth Routes (`/api/auth`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/register` | None | Register parent |
| POST | `/confirm-email-with-code` | None | Confirm email with 6-digit code |
| POST | `/login` | None | Parent login |
| POST | `/logout` | Parent | Logout |
| POST | `/refresh-token` | None | Refresh JWT |
| POST | `/forgot-password` | None | Send reset code |
| POST | `/reset-password-with-code` | None | Reset password |
| POST | `/request-email-change` | Parent | Request email change |
| POST | `/confirm-email-change-with-code` | None | Confirm email change |
| GET | `/my-profile` | Parent | Get profile |
| PUT | `/profile` | Parent | Update profile |
| GET | `/my-dashboard` | Parent | Dashboard with children |
| GET | `/child/{childId}/card` | Parent | Child detailed card |
| DELETE | `/delete-parent-account` | Parent | Soft delete account |
| POST | `/recover-account` | None | Recover deleted account |

### Children Routes (`/api/children`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/complete-initial-profiling` | Parent | Create child + questionnaire |
| POST | `/login-with-code` | None | Child login |
| POST | `/refresh-token` | None | Child token refresh |
| POST | `/logout` | Child | Child logout |
| POST | `/add-existing` | Parent | Link existing child |
| GET | `/my-children` | Parent | List parent's children |
| GET | `/my-profile` | Child | Child profile |
| GET | `/my-dashboard` | Child | Child dashboard |
| PUT | `/{childId}` | Parent | Update child info |
| POST | `/{childId}/regenerate-code` | Parent | New login code |
| DELETE | `/{childId}` | Parent | Delete child |
| DELETE | `/{childId}/unlink` | Parent | Unlink child from parent |

### Allowance Routes (`/api/allowance`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/{childId}/balance` | Parent | Child balance |
| GET | `/{childId}` | Parent | Allowance settings |
| PUT | `/{childId}` | Parent | Set/update allowance |
| POST | `/{childId}/bonus` | Parent | Give one-time bonus |
| PUT | `/bonus/{transactionId}` | Parent | Edit bonus |
| GET | `/{childId}/transactions` | Parent | Transaction history |
| GET | `/my-balance` | Child | My balance |
| GET | `/my-transactions` | Child | My transaction history |
| GET | `/my-allowance` | Child | Allowance info |

### Expense Routes (`/api/expense`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/log` | Child | Log expense |
| PUT | `/{expenseId}` | Child | Edit expense |
| GET | `/my-expenses` | Child | My expense history |
| GET | `/{childId}/expenses` | Parent | Child expense history |
| GET | `/categories` | Both | List categories |
| GET | `/moods` | Both | List moods |

### Goal Routes (`/api/goal`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/personal` | Child | Create personal goal |
| GET | `/my-goals` | Child | My goals |
| POST | `/{goalId}/add-money` | Child | Add money to goal |
| PUT | `/personal/{goalId}` | Child | Edit personal goal |
| DELETE | `/personal/{goalId}` | Child | Delete personal goal |
| POST | `/{childId}/challenge` | Parent | Create challenge |
| GET | `/{childId}/goals` | Parent | Child's goals |
| PUT | `/{childId}/challenge/{goalId}` | Parent | Edit challenge |
| DELETE | `/{childId}/challenge/{goalId}` | Parent | Delete challenge |

### Quiz Routes (`/api/quiz`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/next` | Child | Get next question |
| POST | `/submit` | Child | Submit answer |

### Report Routes (`/api/report`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/{childId}/summary` | Parent | Spending summary |
| GET | `/{childId}/category-breakdown` | Parent | Category breakdown |
| GET | `/{childId}/mood-spending` | Parent | Mood vs spending |
| GET | `/{childId}/time-patterns` | Parent | Time patterns |
| GET | `/{childId}/balance-history` | Parent | Balance history |
| GET | `/{childId}/goal-report` | Parent | Goal report |
| POST | `/{childId}/download-pdf` | Parent | Download PDF report |

### Other Routes
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/character/available` | None | List characters |
| PUT | `/api/character/select` | Child | Select character |
| POST | `/api/character/state` | Child | Get character state for screen |
| GET | `/api/character/profile-picture` | Child | My profile picture |
| GET | `/api/character/{childId}/profile-picture` | Parent | Child's profile picture |
| GET | `/api/achievement/my-achievements` | Child | Badge trophy case |
| GET | `/api/insight/{childId}/key-insights` | Parent | Key insights |
| GET | `/api/insight/fun-facts` | Child | Fun facts |
| GET | `/api/analysis/{childId}` | Parent | Full analysis + advice cards |
| POST | `/api/chatbot/child` | Child | Child chatbot |
| POST | `/api/chatbot/parent` | Parent | Parent chatbot |
| GET | `/api/notification/my` | Both | My notifications |
| GET | `/api/notification/unread-count` | Both | Unread count |
| PATCH | `/api/notification/{id}/read` | Both | Mark as read |
| PATCH | `/api/notification/read-all` | Both | Mark all as read |
| POST | `/api/contact-us` | None | Contact form |

---

## Authentication

The API uses JWT Bearer tokens. After login, include the token in every request:

```
Authorization: Bearer <your_access_token>
```

- **Parent token** — obtained from `POST /api/auth/login`
- **Child token** — obtained from `POST /api/children/login-with-code`
- **Refresh** — use `POST /api/auth/refresh-token` (parent) or `POST /api/children/refresh-token` (child) before the access token expires (15 minutes)

---

## Real-Time Notifications (SignalR)

Connect to the hub after login:

```
wss://localhost:7048/hubs/notifications
```

Include the JWT token in the connection. The hub automatically adds the connection to the correct group (`parent-{id}` or `child-{id}`).

Listen for the `ReceiveNotification` event:

```js
connection.on("ReceiveNotification", (notification) => {
  console.log(notification.title, notification.message);
});
```

---

## Background Jobs (Hangfire)

Scheduled jobs run automatically:

| Job | Schedule | Description |
|---|---|---|
| Credit allowances | Every 15 min | Credits due allowances to children |
| Fail expired goals | Daily 1 AM | Marks overdue goals as failed, refunds balance |
| Update children ages | Daily 2 AM | Recalculates ages from DOB |
| Permanent account deletion | Daily 3 AM | Anonymizes accounts past 30-day grace period |
| Daily expense reminders | Daily 4 PM | Notifies children who haven't logged today |
| Weekly personality update | Saturday 4 AM | Recalculates personality scores via AI |

Monitor and trigger jobs manually at `/hangfire`.

---

## Personality Types

| Key | Parent Label | Child Label |
|---|---|---|
| `IMPULSIVE_SPENDER` | Impulsive Spender | Speedy Spender |
| `PRUDENT_SAVER` | Prudent Saver | Treasure Keeper |
| `GOAL_ORIENTED_PLANNER` | Goal-Oriented Planner | Dream Builder |
| `BARGAIN_HUNTER` | Bargain Hunter | Deal Detective |

Children start with "Pending Analysis" and get a real personality assigned after the initial questionnaire (via AI) and weekly thereafter.

---

## Characters

Four space-themed characters children can choose from:

| ID | Name | Description |
|---|---|---|
| 1 | Nova | Cool astronaut who loves street style and music |
| 2 | Cosmo | Ninja superhero astronaut always ready for action |
| 3 | Luna | Graceful ballerina astronaut in a pink skirt |
| 4 | Stella | Laid-back astronaut in a hoodie who loves bubblegum |

Character images are served by the backend from `/wwwroot/images/characters/`. Request the right image per screen using `POST /api/character/state` with a `screenContext` value (`Dashboard`, `Expenses`, `Goals`, `Profile`, `Badges`, `Quiz`).

---

## Environment Variables Summary

### Backend (`appsettings.json`)
| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Jwt:SecretKey` | JWT signing key (min 32 chars) |
| `AIService:Url` | URL of the Python Flask AI service |
| `SendGrid:ApiKey` | Brevo/SendGrid API key |
| `QuizImport:JsonFilePath` | Path to quiz JSON file |
| `QuizImport:ImportKey` | Secret key for the quiz import endpoint |

### AI Service (`.env`)
| Key | Description |
|---|---|
| `GEMINI_API_KEY` | Google Gemini API key (primary LLM) |
| `GROQ_API_KEY` | Groq API key (fallback LLM) |
| `OPENROUTER_API_KEY` | OpenRouter API key (second fallback) |

---

## Notes for Frontend

- All API responses are wrapped in `{ success, message, data, errors }`.
- Dates are in UTC. Display them converted to the user's local timezone.
- Child login uses a 6-character alphanumeric code (e.g. `ABC123`), not email/password.
- The `ChildId` claim in the child JWT is accessed via the token — no need to pass it in request bodies for child-scoped endpoints.
- For the chatbot endpoints, send the last 3 messages as `history` (trimmed on your side before sending).
- Character images are relative paths — prepend the API base URL to construct the full image URL.
- SignalR requires the JWT token to be passed during hub connection for group assignment to work.

---
 
## Contributors
 
| Name | Role | GitHub |
|---|---|---|
| Jannah Ayman | Backend | [github.com/](https://github.com/jannah-ayman) |
| Nancy Saad | Backend | [github.com/](https://github.com/nancyabdelbaryy) |
| Rawan Sotohy | AI | [github.com/](https://github.com/Rawan-Sotohy) |
| Rahma Tarek | AI  | [github.com/](https://github.com/Rahma-Tarek52004) |
| Doha Emad | Frontend | [github.com/](https://github.com/Doha-2004) |
| Arwa Hassan | Frontend  | [github.com/](https://github.com/ArwaHassan2312) |