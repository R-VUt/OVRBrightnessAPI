# OVRBrightnessAPI

This code is a simple application that allows you to control the brightness of an overlay using the OVRSharp library. It provides an HTTP server that listens for requests to get and set the brightness value.

## Requirements

- OVRSharp library
- Valve.VR library

## Usage

1. Make sure you have the OVRSharp and Valve.VR libraries referenced in your project.
2. Build and run the application.
3. The HTTP server will start listening on http://localhost:13902/.
4. Use the following endpoints to interact with the application:

   - `/brightness` (GET): Retrieves the current brightness value.
   - `/brightness/set` (POST): Sets the brightness value. The new brightness value should be included in the request body as a float value between 0 and 1.

## Example

To retrieve the current brightness value:
```
GET http://localhost:13902/brightness
```

To set the brightness value to 0.5:
```
POST http://localhost:13902/brightness/set
Body: 0.5
```

**Note:** The brightness value should be between 0 and 1, where 0 represents the minimum brightness and 1 represents the maximum brightness.