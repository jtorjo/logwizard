### Log Wizard - a Log Viewer that is easy and fun to use!

I created **LogWizard** to help anyone that really needs to deal with logs, in order to _hunt down bugs and/or issues that happen somewhere else_ (read = at customer site).

My focus has been on ease of use from the get go. When using LogWizard, most things should be easy. In case they're not - I've done something wrong :) Let me know!


***

Since 1.6, I've made Log Wizard work with Windows Event Logs as well. However, on 1.7 I've done quite a few improvements to speed things up/help you on Event Logs.

***

To get started on Windows Event Logs, here's a sneak peak:

Press **Ctrl-O** (equivalent of Actions >> Open Log). Select "Windows Event Log".

![LogWizard](https://github.com/jtorjo/logwizard/blob/master/src/images/el01.png)

Press OK

Events will start coming in - as they show up, you'll see the progress in the status bar. Depending on the size of your Event Logs, this may take a few seconds, but the UI is responsive at all times.

![LogWizard](https://github.com/jtorjo/logwizard/blob/master/src/images/el02.png)


By default, I will show you events from the **Application** and the **System** Event Logs bundled together. The latest events come first - thus you can have a quick look at what happened last while still waiting for the older events.

You can tweak the above quite a bit to read other Event Logs. 

Read more about it, on [Codeproject](http://www.codeproject.com/Articles/1073292/Log-Wizard-Viewing-Windows-Event-Logs-Can-Be-Fun)
* <a href="http://www.codeproject.com/Articles/1073292/Log-Wizard-Viewing-Windows-Event-Logs-Can-Be-Fun" target="_blank">Windows Event Logs</a>
