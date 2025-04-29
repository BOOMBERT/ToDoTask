# 📝 Description

__ToDoTask__ is a robust __REST API__ designed using __Clean Architecture__ principles and the __CQRS__ pattern. 
It enables efficient task management, allowing users to create, update, delete, and retrieve ToDo items, with powerful support for pagination, filtering, and sorting. 
Additionally, users can set and update the completion percentage and specify an expiry date for each task. 

The API handles various __time zones__ seamlessly, ensuring that users can manage tasks across different locations and time settings.

With a clean, well-structured design and a clear separation of concerns, the __API__ is both scalable and easy to maintain. 
It also includes comprehensive __unit__ and __integration tests__, ensuring the reliability and correctness of both the business logic and the __API__ endpoints. 

# ✨ Features

## 🔹 Time Zone Support

The __API__ operates entirely in __UTC__, ensuring consistency when creating and returning tasks. 
When filtering tasks by date, users must provide their __IANA Time Zone ID__ (e.g., `America/Los_Angeles`, `Europe/Warsaw`). 
This enables the __API__ to apply date operations accurately based on the user's local timezone.

## 🔹 Filtering Tasks By Date Range

The provided time zone is specifically used to filter tasks by expiry date, allowing users to retrieve tasks scheduled for `Today`, `Tomorrow`, or `This Week` in their local time context. 
The __API__ calculates the appropriate __UTC__ boundaries based on the user's time zone and applies them during query execution.

\
To easily retrieve the client’s time zone in __JavaScript__, use: `Intl.DateTimeFormat().resolvedOptions().timeZone`

# 🛠️ Technologies
- __Backend__: .NET 8 (C#), ASP.NET Web API, MediatR
- __Database__: PostgreSQL, Microsoft.EntityFrameworkCore
- __Validation__: FluentValidation
- __Testing__: xUnit, Moq, SQLite

# 💻 Run Locally

- __Prerequisites:__
	- Git
	- Docker

- __Setup:__
	- _1. Clone the repository:_
		```bash		
	    git clone https://github.com/BOOMBERT/ToDoTask.git
		```
	- _2. Navigate to the project directory:_
		```bash			
	    cd ToDoTask
		```
	- _3. Build and run the API:_
		```bash	
	    docker-compose up --build
		```
