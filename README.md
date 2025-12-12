# High-Performance Order Processor  
A .NET 6 Windows Background Service that processes JSON orders dropped into an `IncomingOrders` folder, applies validation & business rules, and stores results in a SQLite database.  
Includes a configurable Order Generator Tool for testing high-volume bursts, invalid data, and corrupted JSON files.

---

##  Features

###  Windows Background Service  
- Built using `.NET 6` Worker Service (`BackgroundService`)
- Runs as console during development  
- Supports installation as **Windows Service**  
- Gracefully shuts down via `CancellationToken`

###  File Ingestion Pipeline  
- Watches `IncomingOrders/` folder using **FileSystemWatcher**  
- Event-driven (no polling allowed)  
- Handles:
  - High-volume bursts (1000+ files)
  - Locked / partially written files  
  - Duplicate file prevention (**idempotency**)

###  Order Validation Rules  
- Invalid if `TotalAmount < 0`  
- Invalid if `CustomerName` is missing or empty  
- Business Rule: `TotalAmount > 1000` → Mark as **HighValue**

### SQLiteDatabase Persistence  
**ValidOrders Table**
- OrderId  
- CustomerName  
- OrderDate  
- Items  
- TotalAmount  
- IsHighValue  

**InvalidOrders Table**  
- Raw JSON  
- Reason for failure  

###  JSON File Generator Tool  
Generates:  
- Valid orders  
- Invalid orders  
- Corrupted JSON  
- High-volume bursts (1000+ files)  
- Fully configurable via CLI parameters  

###  Logging & Resiliency  
- Structured logs (ILogger)  
- Service never crashes — continues after:  
  - Database busy  
  - JSON corruption  
  - Locked files  
  - Input load spike  

---

##  Project Structure

```
HighPerformanceOrderProcessor/
│
├── OrderProcessorService/     # Main Worker Service
│   ├── Program.cs
│   ├── Worker.cs
│   ├── Services/
│   │       FileWatcherService.cs
│   │       OrderProcessor.cs
│   │       DatabaseService.cs
│   ├── Models/
│   │       Order.cs
│   ├── appsettings.json
│   └── orders.db
│
├── OrderGeneratorTool/        # JSON file generator tool
│   └── Program.cs
│
├── SampleOrders/
│       valid.json
│       invalid.json
│       corrupted.json
│
├── Tests/
│       OrderProcessorTests.cs
│
└── README.md
```

---

##  Setup Instructions

###  Prerequisites  
- Windows 10/11  
- .NET 6 SDK  
- SQLite (or DB Browser for SQLite)  

---

##  Running the Service (Development Mode)

```bash
cd OrderProcessorService
dotnet run
```

You should see:

```
Order Processor Service started
New file detected: order_1234.json
Order saved: 71b2f9f1...
```

---

##  Installing as a Windows Service

### Step 1: Publish the project
```bash
dotnet publish -c Release
```

### Step 2: Install the service  
Run PowerShell as **Administrator**:

```powershell
sc create OrderProcessorService binPath= "C:\path\to\publish\OrderProcessorService.exe"
sc start OrderProcessorService
```

To stop:

```powershell
sc stop OrderProcessorService
```

To delete:

```powershell
sc delete OrderProcessorService
```

---

##  Testing the System

Use the JSON generator tool.

### Generate 10 files:
```bash
dotnet run --project OrderGeneratorTool 10
```

### Generate 1000 files:
```bash
dotnet run --project OrderGeneratorTool 1000
```

### Expected behaviors:
 Valid orders saved to ValidOrders table  
 Invalid/corrupted orders saved with reason  
 No duplicate processing (idempotency)  
 Logs for:
- New file detected  
- Processing started  
- Saved to DB  
- Validation failure  
- Exceptions handled gracefully  

---

##  How Idempotency Is Implemented

A thread-safe dictionary tracks processed file paths:

```csharp
ConcurrentDictionary<string, bool> _processed;
```

When a file appears:
- If already processed → ignored  
- If new → marked as processed → executed  

This ensures:
- Restart-safe  
- No double-processing  
- Consistency in burst conditions  

---

## Automated Tests

Located in `Tests/`.

Includes:
- Invalid amount test  
- Corrupted JSON test  
- Valid order test  

Run tests:

```bash
dotnet test
```

---

## Sample Output Screenshots (Recommended for Submission)

Include these in your GitHub:
1. Service detecting new file  
2. ValidOrders table (SQLite)  
3. InvalidOrders table  
4. Generator tool CLI output  
5. 1000-file burst processing  
6. Idempotency demonstration  

---

##  Improvements With More Time

Given additional development time, I would enhance the project by:

**Implementing a Threaded Queue-Based Processing Pipeline**, improving:
- Throughput  
- Fault tolerance  
- Prioritization of high-value orders  

Other future improvements:
- Docker support  
- REST API for order submission  
- Dashboard monitoring (Grafana/ELK)  
- Retry handling with exponential back-off  

---

##  Author  
**Poongulali G**,  
B.Tech Information Technology  
High-performance backend development & system architecture enthusiast.

---

##  License  
This project is released under MIT License.

