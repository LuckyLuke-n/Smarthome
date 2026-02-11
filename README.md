# Smarthome
Welcome to the SmartHome application! This application allows you to monitor and manage the environmental conditions of your home and surrounding area. With real-time data from sensors and a third-party API, you can keep track of temperature, humidity, pressure, and more.
<img width="1659" height="1037" alt="image" src="https://github.com/user-attachments/assets/78190206-931b-40e5-a0b1-6593567bf3c5" />

## Features
- Indoor Monitoring: Displays real-time temperature and humidity levels measured by indoor sensors.
- Outdoor Monitoring: Fetches and displays outdoor temperature, humidity, pressure, and other environmental values for a specified location using a third-party API.
- User-Friendly Interface: Easy-to-navigate interface for quick access to all monitored data.
- Custom Location Settings: Allows users to set their preferred location for outdoor weather data.

## Technologies Used
- Frontend: Grafana for visualizing and displaying the data.
- Backend:
  - Developed using .NET for server-side functionality.
  - EMQX as a MQTT message bus
  - MongoDB as NoSQL storage
  - Prometheus for metrics
  - OpenTelemetry collector
  - NGINX as reverse proxy
- API Integration: Utilizes the [tomorrow.io](https://tomorrow.io) API to retrieve outdoor weather data.
- Sensors: Compatible with BME280 sensors for measuring temperature, humidity, and pressure indoors.

## Usage
- Indoor Data: The application will display the current temperature and humidity from the connected sensors.
- Outdoor Data: Enter your desired location (city name or coordinates) to fetch the latest outdoor weather data.
