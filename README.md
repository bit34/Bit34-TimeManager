# **Bit34 Time Manager Library**

# **Table of contents**
- [What is it?](#what-is-it)
- [Who is it for?](#who-is-it-for)
- [Basic example](#basic-example)

## **What is it?**
This is a minimal time library with scheduled callbacks. 
With this library you can;

- Add callbacks to be called every application tick
- Add callbacks to be called after given interval
- Auto remove callbacks after given call count
- Pause, resume and remove callback individualy or by their owner key

There are 3 type of times that you can use
- UTC
- Application (starts with application)
- ApplicationScaled (starts with application, can be scaled)

## **Who is it for?**
You can use this library for your basic timing and schedule needs without boiler plate code.

## **Basic example**
```
//  Tick will be called 60 times per second
float tickInterval = 1.0f / 60.0f;

//  You will be able to use scaled time at half the normal speed
float timeScale = 0.5f;

//  You need a class that implementes ITime for that platform, You can use DefaultTime for most of the cases
ITime  time = new DefaultTime(tickInterval, timeScale);

//  Then you can create TimeManager with yout ITime implementation
TimeManager timeManager = new TimeManager(time);

timeManager.Scheduler.AddTick(this, TimeTypes.Utc, UtcTickLCallback);

.
.
.

void UtcTickLCallback(float timeStep)
{
    Console.WriteLine("UTC Time:" + time.GetNow(TimeTypes.Utc).ToString("HH:mm:ss:fff"));
}
```

