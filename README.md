### Log Wizard - a Log Viewer that is easy and fun to use!

[![Join the chat at https://gitter.im/jtorjo/logwizard](https://badges.gitter.im/jtorjo/logwizard.svg)](https://gitter.im/jtorjo/logwizard?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Build status](https://ci.appveyor.com/api/projects/status/566ksg8oh95bj0o0?svg=true)](https://ci.appveyor.com/project/jtorjo/logwizard) [![GitHub issues](https://img.shields.io/github/issues/jtorjo/logwizard.svg)](https://github.com/jtorjo/logwizard/issues)

![LogWizard](https://github.com/jtorjo/logwizard/blob/master/src/images/lw_demo.gif)

I created **LogWizard** to help anyone that really needs to deal with logs, in order to _hunt down bugs and/or issues that happen somewhere else_ (read = at customer site).

My focus has been on ease of use from the get go. When using LogWizard, most things should be easy. In case they're not - I've done something wrong :) Let me know!

***

Want to download the **latest release**? Head over [here!](https://github.com/jtorjo/logwizard/releases)

Read about **LogWizard** on on <a href="http://www.codeproject.com" target="_blank">CodeProject</a> : 
* <a href="http://www.codeproject.com/Articles/1023815/LogWizard-a-Log-Viewer-that-is-easy-and-fun-to-use" target="_blank">General (Introduction)</a> 
* <a href="http://www.codeproject.com/Articles/1078310/Log-Wizard-Make-your-Logs-Look-Pretty" target="_blank"><b>Pretty Formatting! (v1.8+)</b></a>  (2nd Best C# Article/Feb 2016)
* <a href="http://www.codeproject.com/Articles/1045528/LogWizard-Filter-your-Logs-Inside-out" target="_blank">Easy creation of Filters/Views</a> 
* <a href="http://www.codeproject.com/Articles/1039389/LogWizard-Talk-About-your-Logs" target="_blank">Create/Share Notes on Logs</a>
* <a href="http://www.codeproject.com/Articles/1073292/Log-Wizard-Viewing-Windows-Event-Logs-Can-Be-Fun" target="_blank">Windows Event Logs</a>

***

My team and I have created a rather large piece of software that is running on thousands of machines every day. When our customers encounter an issue, they send us their logs. The software is pretty big, we run 10+ threads and log a lot of information. Focusing on a certain issue (the customer's) has always been rather complicated. And yes, we tried other Log Viewers, but lets just say they were not up to the task.

Here are some of LogWizard's features:
- **Easy to filter information** - easy to create filters, easy to turn on/off, easy to copy/paste/modify.
- [**Pretty Formatting! (v1.8+)**](http://www.codeproject.com/Articles/1078310/Log-Wizard-Make-your-Logs-Look-Pretty) - You can have the information that matters to you most to just stand out of the crowd!
- [**nlog / log4net Enhanced Support**](https://github.com/jtorjo/logwizard/wiki/loglibraries) - It guesses the log (file/database) syntax from the nlog/log4net .config files
- **Coloring** - allow a filter to have a certain color - allow you visually identify important information (color-the-full-line or color just-what-matches). You can already see coloring in action in the first image above.
- **Easy to switch** from "My filtered view" to the "Full log" and back (**Alt-L**).
- **Easy to switch** between logs. All logs you open are kept in History. Switching between them is bliss - **Ctrl-H**.
- **Ease of use** - once you've set up your filters, getting to the information you care about is a piece of cake!
- **Hotkeys! Hotkeys! Hotkeys!** I'm a developer - mouse is too slow. I want to switch between views/logs/ toggle views on/off, whatever - just with [Hotkeys](Hotkeys).
- **Real-time monitoring** - drag and drop a file, and monitor it live, as your program is writing to it
- **Windows Event Logs** - as of version [1.6](https://github.com/jtorjo/logwizard/releases/tag/1.6.1), you can very easy view `Window Event Logs`. Just hit **Ctrl-O**, select `Windows Event Log` from the combo, and that's it! _It even works for viewing remote event logs!_
- **Debug Viewer** - as of version [1.6](https://github.com/jtorjo/logwizard/releases/tag/1.6.1), you can view anything your program outputs via `OutputDebugString`. You can view the information from all programs, or filter it just by your program name. Hit **Ctrl-O**, and select `DebugPrint`.
- **Database Support**. Can read from common databases, such as MSSQL, Oracle, SQLite. Bonus - drop an Sqlite database onto Logwizard and it automatically guesses the log table + log fields
- **View Summary** - show me how many lines a certain view has. For example, I have View that shows me notifications, errors, and fatal errors. When I open a log, the first thing I check is - how many lines are in that view? If too many, that's the first View I look at.
- **Remember my settings**. I don't want to have to specify the same thing twice. Once - then reuse it for as many logs as you want!
- **Auto-saving** - you don't need to save anything. Everything you set is automatically saved by default
- **Show/Hide/Move Columns** - just right click on the Columns header. You can edit them as easy as possible. And next time you open LogWizard, it remembers what you've done!

There's more:
- **Search-as-you-go** - start typing - LogWizard will take you to the first column that contains the given text, and also highlight the text you typed. And, just use F3/Shift-F3 (Find-Next/Find-Previous), and they will instantly take you to the next/previous occurrence of what you just typed!
- **Smart Find, with Preview** - First off - Find (**Ctrl-F**) can handle regex-es. But more to the point, it will show you a preview of what you would find. This is extremely useful when you're typing a regex, which is so easy to get wrong the first few times...
- **Go to Line/Time** - with a twist! It allows offsets, and understands lots of time formats! Just hit **Ctrl-G**, and enjoy!
- **Clipboard-Friendly** - select several rows, press **Ctrl-C** or **Ctrl-Shift-C**, and they are copied to clipboard, preserving the _colors!_
- **Details pane** - If you have lots of columns, or multi-line columns, or both, we have a Details pane (**Ctrl-D**). You can choose what gets shown there so that it doesn't clog your main View!
- **Show me just what I want** - Yes, we have lots of [Toggles](Toggles)! You can toggle information on/off - so that you get the most of your screen! You can toggle on/off the Details pane - so that it's shown only when you need it. Same for the Filters pane (where you edit your filters) - which most of the time, you want it hidden. There are plenty more you can show/hide, so that you get as much information as possible!

I welcome any feedback you may have, and any suggestions are welcome as well. 

If you discover an issue/problem/bug - please upload the problematic log file somewhere I can download it; probably the easiest place would be [dropbox](http://www.dropbox.com). Then create a github issue, with a link to the uploaded log. I'll do my best to answer ASAP. 
Thanks!
