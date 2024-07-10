# Out of Office Solution

## Project Description

The "Out of Office" solution is an application designed to manage employee leave requests, approvals, and project assignments. It provides different functionalities for HR managers, Project managers, and Employees. The application is built using ASP.NET Core, microservices architecture, and containerized with Docker. It uses RabbitMQ for message handling and Kubernetes for orchestration.

## Tech Stack

- **Backend**: ASP.NET Core
- **Architecture**: Microservices
- **Messaging**: RabbitMQ
- **Containerization**: Docker
- **Orchestration**: Kubernetes
- **Database**: SQL Server

## Features

### HR Manager

- **Employees Management**
  - Sort, filter, and search employees
  - Add, update, deactivate employees
- **Approval Requests Management**
  - Sort, filter, and search approval requests
  - Open, approve, reject requests
- **Leave Requests Management**
  - Sort, filter, and search leave requests
  - Open leave request details
- **Projects Management**
  - Sort, filter, and search projects
  - Open project details

### Project Manager

- **Employees Management**
  - Sort, filter, and search employees
  - Open employee details
  - Assign employees to projects
- **Approval Requests Management**
  - Sort, filter, and search approval requests
  - Open, approve, reject requests
- **Leave Requests Management**
  - Sort, filter, and search leave requests
  - Open leave request details
- **Projects Management**
  - Sort, filter, and search projects
  - Add, update, deactivate projects

### Employee

- **Leave Requests Management**
  - Sort, filter, and search leave requests
  - Create, update, submit, cancel leave requests
- **Projects Management**
  - View assigned projects