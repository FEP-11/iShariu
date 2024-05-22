# iShariu: Online Course Platform

## Overview

Welcome to iShariu! iShariu is an online course platform designed to provide a wide range of courses for students and professionals. Our platform aims to offer interactive learning experiences to enhance your skills and knowledge in various domains.

## Features

- Course Catalog: Explore our diverse catalog of courses covering topics ranging from technology and business to arts and humanities.
- User Authentication: Create an account to enroll in courses, track your progress, and access additional features.
- Interactive Learning: Engage with interactive content, quizzes, and assignments to reinforce your learning.
- Community Interaction: Connect with fellow learners, participate in discussions, and share knowledge.
- Responsive Design: Enjoy seamless learning experiences across devices with our responsive web design.

## Technologies Used

- ASP.NET Core 7.0: Framework for building web applications with Model-View-Controller architecture.
- MongoDB: NoSQL database for storing course information, user data, and other relevant information.
- HTML/CSS/Bootstrap v5.0/JavaScript/jQuery: Frontend technologies for creating user interfaces and interactive elements.
- C#: Backend language for server-side logic and interactions with the database.
- Docker: Containerization platform used to package the application and its dependencies in a virtual container.

## Architecture

This project follows the Model-View-Controller (MVC) architectural pattern. The application is divided into three interconnected parts, allowing for efficient code reuse and parallel development.

## Deployment

The application is containerized using Docker, which ensures that it works uniformly across different computing environments.

## Contributing

We welcome contributions from everyone! If you'd like to contribute to iShariu, please follow these guidelines:

- Fork the repository.
- Create your feature branch (git checkout -b feature/YourFeature).
- Commit your changes (git commit -am 'Add some feature').
- Push to the branch (git push origin feature/YourFeature).
- Create a new Pull Request.

## Acknowledgments

We would like to thank all the contributors who have helped to improve this project.

## Getting Started

To get started with iShariu, follow these steps:

1. Clone the repository to your local machine.

git clone git@github.com:FEP-11/iShariu.git

2. Install Docker on your machine if you haven't already.

3. Set up MongoDB:

   - Install MongoDB on your machine if you haven't already.
   - Start the MongoDB service.
   - Configure the connection string in the appsettings.json file to point to your MongoDB instance.

4. Open the project in JetBrains Rider or your preferred IDE.

5. Build the Docker image and run the Docker container.

docker-compose up --build

6. Open your browser and navigate to http://localhost:45677 to view the website locally.