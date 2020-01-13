# Welcome to SMA Technologies Coding Challenge!

## The Challenge
We want you to code a sort of 'self-driving car'.  There is a REST API that will create a 'road course' for you to drive on.  The road will have speed limits that you will be expected to obey to the best of your ability.  Once you start your vehicle, the server will track the progress of your car and be able to provide you with updates on it as well as the current road condition(s).  Your job is to build the client functionality that controls the car by submitting 'actions' based on this information.

## Getting Started
We have published a public REST API at https://smachallenge.azurewebsites.net.  This API will be the server-side component you interact with as part of this challenge.

The general flow to get started might look like:

- Call the endpoint to register yourself, pick a course, and receive a token that will be used to track your attempt.
- Submit the 'IgnitionOn' action.
- Call the endpoint to get the current rules of the road.
- Submit the 'Accelerate' action.
- Continue submitting 'Accelerate' and 'Brake' actions while getting the current car status and road rules until the end of the course is reached.
- Submit the 'IgnitionOff' action.

There are 3 pre-made courses and they are progessively more complex.  When registering, you may select a course layout (1-3).  The default is 1.

Full documentation on the API is further below.

### General Notes

 1. All measurements are in metric.
 2. The car ignition must be turned on (submit the 'IgnitionOn' car action) to get any car or road information.
 3. The car endpoint will return engine state, current speed, total distance traveled, and total time traveled.
 4. The road endpoint will return current maximum speed, current minimum speed, and if there is a speed limit change within the next 200 meters it will provide the future maximum speed, future minimum speed, and distance to reach that future speed limit.
 5. Maximum speed is 40 m/s.  Minimum speed is 0 m/s.
 6. Maximum acceleration (or deceleration) force is 6 m/s^2.  When using the 'Brake' car action, your 'Force' is submitted as a positive number.
 7. If the car is approaching a stop sign, it will have a future speed limit maximum and minimum of 0 m/s.  It will be this way for 50 meters.  Once in the zone, the future speed limit will return to a standard speed limit with the new speed.
 8. When in the 'stop sign zone', come to a complete stop.  Then, accelerate to leave the 50 meter zone and return to normal speed limits.
 9. If the current speed limit is 0 and there is no future speed limit, you are at the end of the course.  Submit the 'IgnitionOff' command.  This will erase settings for the car.
 10. All interactions with the REST API are logged.

### Coding Guidelines

 1. Any object-oriented language is acceptable, but it would score major points to do it in C# and .NET Core.
 2. We want people who like writing unit tests.  So design your code to be testable, and write the best unit tests you can.  If you do TDD - perfect!
 3. Submit clean code and try to adhere to S.O.L.I.D. principles - this is a test.
 4. The more efficient your code is (don't accidentally Denial-of-Service attack us, please), the better.
 5. The faster you finish, the better.

### Submission Guidelines

 1. Zip all your files into a single archive file.
 2. Include all source code including unit test projects.
 3. Include all binaries including dependent libraries.
 4. You may also submit your source repository, if you have used one to develop your solution.
 5. Include a "Readme" file (e.g. Text, Word, Markdown) with instructions on how to run your application.
 6. Contact us for how to deliver the archive file.

### Sample

This repository contains a sample to get you started.  It contains the entity definitions and a basic REST client for interacting with the REST API.  

A typical implementation of these libraries might look like:

    ISelfDrivingCarService client = new SelfDrivingCarRestClient();
    client.Register(new TokenRequest { Name = "sampleapplicant@email.com" });
    CarAction actionResult = client.DoAction(new CarAction { Action = "IgnitionOn" });
    Road road = client.GetRoad();
    ...


Feel free to use as much or as little of it as possible.  Remember that even if you use our sample, we want testing!

### API Reference

**api/tokens**

Method: **POST**

Request:

    {
        "name": "sampleapplicant@email.com",
        "courseLayout": 3
    }

**Note:** courseLayout is optional.  Default is 1.

Response:

    {
        "name": "sampleapplicant@email.com",
        "courseLayout": 3
    }

**api/road**

Method: **GET**

Request: *Empty*

Response:

    {
        “currentSpeedLimit”: { “min”: 15, “max”: 25 },
        “speedLimitAhead”: { “min”: 30, “max”: 40, “remainingDistanceToEnforcement”: 50.534 }
    }

**api/car**

Method: **GET**

Request: *Empty*

Response:

    {
        “ignition”: “On”,
        “engine”: { “state”: “Running” },
        “currentVelocity”: 15.265,
        “totalDistanceTraveled”: 281.328,
        “totalTimeTraveled”: 36.461
    }

**api/carActions**

Method: **POST**

Request: 

    {
        “action”: “IgnitionOn”
    }

or

    {
        “action”: “Accelerate”,
        “force”: 3
    }

Response:

    {
        “action”: “Accelerate”,
        “force”: 3
    }
