## LogWizard - a Log Viewer that is easy and fun to use!

[![Join the chat at https://gitter.im/jtorjo/logwizard](https://badges.gitter.im/jtorjo/logwizard.svg)](https://gitter.im/jtorjo/logwizard?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)  [![Build status](https://ci.appveyor.com/api/projects/status/566ksg8oh95bj0o0?svg=true)](https://ci.appveyor.com/project/jtorjo/logwizard) [![GitHub issues](https://img.shields.io/github/issues/jtorjo/logwizard.svg)](https://github.com/jtorjo/logwizard/issues)


![LogWizard](https://github.com/jtorjo/logwizard/blob/master/images/00shot_first.png)

I created **LogWizard** to help anyone that really needs to deal with logs, in order to _hunt down bugs and/or issues that happen somewhere else_ (read = at customer site).

***

Read about **LogWizard** on on <a href="http://www.codeproject.com" target="_blank">CodeProject</a> : 
* <a href="http://www.codeproject.com/Articles/1023815/LogWizard-a-Log-Viewer-that-is-easy-and-fun-to-use" target="_blank">General (Introduction)</a> 
* <a href="http://www.codeproject.com/Articles/1045528/LogWizard-Filter-your-Logs-Inside-out" target="_blank">Easy creation of Filters/Views</a> 
* <a href="http://www.codeproject.com/Articles/1039389/LogWizard-Talk-About-your-Logs" target="_blank">Create/Share Notes on Logs</a>

***

My team and I have created a rather large piece of software that is running on thousands of machines every day. Our customers, when they encounter an issue, send us their logs. The software is pretty big, we run 10+ threads, we log a lot of information, and parsing through it is pretty complex. Focusing on a certain issue (the customer's) has always been rather complicated. And yes, we tried other Log Viewers, but lets just say they were not up to the task.

So, in the last year or so, whenever I had some spare time, I would work on this. What I wanted was more or less this:
- **Easy filters** - easy to create, specify colors, easy to turn on/off, easy to copy/paste/modify
- **Line coloring** - allow a certain filter to have a certain color - allow you visually identify important lines
- **Easy to switch** from "My filtered view" to the "Full log" and back
- **Easy to switch** between views of a certain log
- **Easy to switch** between logs (all opened logs are kept in History. Switching between them is bliss)
- **Show me as much information as possible**. In other words, once I have defined my filters, I want to forget about them, and see the "View" that they produce, uncluttered by anything else
- **Ease of use** - once you've set up your filters, getting to the information you care about should be a piece of cake!
- **Hotkeys! Hotkeys! Hotkeys!** I'm a developer - mouse is too slow. I want to switch between view/logs/ toggle views on/off, whatever - just with hotkeys
- **Real-time monitoring** - drag and drop a file, and monitor it live, as your program is writing to it
- **View Summary** - show me how many lines a certain view has. For example, I have View that shows me notifications, errors, and fatal errors. When I open a log, the first thing I check is - how many lines are in that view? If too many, that's the first thing I check
- **Remember my settings**. I don't want to have to specify the same thing twice. Once - then reuse it for as many logs as you want!
- **Auto-saving** - you don't need to save anything. Everything you set is automatically saved by default

The driving force for this project has been for me - _**allow you to do anything that is log-related FAST. Find out what you want from a log - as fast as humanly possible**_. Remove the burden of analyzing log files.

I hope I've achieved that - but of course, you'll be the judge. Having said that, LogWizard is BETA. I welcome any feedback you may have, and any suggestions are welcome as well. 

When writing to me, if you've discovered an issue/problem/bug - please attach log files where you've encountered it (no more than 1Mb per email please). I'll do my best to answer ASAP. Thanks!
