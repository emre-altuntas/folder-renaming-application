# Folder Renaming Application
This application is a console application that renames folders using C# 11.0.

Working Principle:

First, the path of the main folder containing the folders to be renamed, the path of the .txt file where each line contains the old and new names of the folders, and the path of the log file are provided.
Each line of the .txt file should be in the format of "old name \t new name."

The process of renaming the folders begins. During the renaming process, an '_' character is appended to the end of each new name to ensure that the new name already exists. Later, this added character is removed upon reading again.
