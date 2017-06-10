# KeyboardBrightness
Controls the keyboard backlight of Samsung Notebook 9 (2017)

Samsung's Notebook 9 (2017 model) has a white keyboard. This is problematic when the keyboard backlight has been set to low and the ambient light is dim (e.g., evening), because the backlight actually makes the key invisible. Of course, you can change keyboard backlight by pressing fn + F9, but doing so everytime is cumbersome. Wouldn't it be nice there was a way to automaate changing the keybord backlight (e.g., turns on at 8 P.M., off at 5 A.M.)? Unfortunately, Samsung's Settings application does not provide ways do that.

To solve the problem, I looked into the directory and guessed the DLL that the Settings application was using. After using some basic tools, I extracted the function names inside of it, and guessed the uses of some of them. With that information, I created this simple application.

To change the backlight, just pass the brightness as an argument. On my model, 0 = off, 1 = low, 2 = medium, and 3 = high. See the code for more details.

This application requires that the Samsung Settings application should be installed, and if you have installed it into a custom path, that path should be specified in the App.config file.
