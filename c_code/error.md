# Debugger 

Instructions to use a debugger to check for bugs in C code on macOS using macPort for Clang: 
1. Compile your C code with debugging symbols. This is important because the debugger needs the debugging symbols to provide accurate information about the code. Here is an example command to compile a C file named mycode.c: 

- clang -g -o main main.c  
 
The -g flag tells the compiler to include debugging symbols in the executable file. 

2. Start the debugger. Use the following command to start the debugger: 

- lldb main  
 
This will start the LLDB debugger and load your code into it. 

3. Set a breakpoint. Use the breakpoint set command to set a breakpoint at a specific line of your code. For example, to set a breakpoint at line 10 of mycode.c, use the following command: 

- breakpoint set --file main.c --line 10  
 
This will set a breakpoint at line 10 of main.c. 
4. Start the program. Use the run command to start the program:

- run  
 
## This will start the program and stop at the breakpoint you set in step 3. 

Debug the program. Once the program has stopped at the breakpoint, you can use the following commands to debug it: 
1. list: Show the current source code around the current breakpoint.
2. next: Step over the current line of code.
3. step: Step into the current line of code.
4. print: Print the value of a variable or expression.
continue: Continue running the program until the next breakpoint or the end of the program. 

For example, you can use the print command to check the value of a variable at a specific point in the code: 

- print my_var  
 
This will print the value of the my_var variable. 
6. Fix the bugs. Once you have identified the bugs in your code, you can fix them by making changes to the code and recompiling it with the -g flag. 
7. Exit the debugger. Use the quit command to exit the debugger: 

- quit  
 
By following these steps, you can use a debugger to check for bugs in your C code on macOS using macPort for Clang.


# Valgrind 
Instructions to use Valgrind to check for memory leaks in C code on macOS using macPort: 
Install Valgrind using macPort. Open your terminal and type the following command: 

- sudo port install valgrind-macos-devel
 
This will install Valgrind on your macOS system. 
Compile your C code with debugging symbols. This is important because Valgrind needs the debugging symbols to provide accurate information about memory leaks. Here is an example command to compile a C file named mycode.c: 

- clang -g -o mycode mycode.c  
 
The -g flag tells the compiler to include debugging symbols in the executable file. 
Run your code with Valgrind. Use the following command to run your code with Valgrind: 

- valgrind --leak-check=full ./mycode  
 
The --leak-check=full flag tells Valgrind to check for all possible memory leaks. 
Analyze the output of Valgrind. Valgrind will produce a lot of output on the terminal. Look for lines that start with definitely lost, indirectly lost, or possibly lost. These indicate memory leaks that need to be fixed. 

```For example, if Valgrind reports: 
==1234== 24 bytes in 1 blocks are definitely lost in loss record 1 of 10  
==1234==    at 0x4C2AB8F: malloc (in /usr/lib/system/libsystem_malloc.dylib)  
==1234==    by 0x100000F6E: main (mycode.c:5)  
 ```

## This means that 24 bytes of memory were allocated with malloc() on line 5 of mycode.c, but were not freed before the program exited. 

Take note of the line numbers and functions mentioned in the Valgrind output. These will help you locate the memory leaks in your code. 
Fix the memory leaks. Once you have identified the memory leaks, you can fix them by making sure that all memory allocated with malloc(), calloc(), or realloc() is freed with free() before the program exits. 

For example, to fix the memory leak in the previous example, you would add the following line at the end of your main() function: 
free(ptr);  
 
-- Where ptr is the pointer returned by malloc(). 

By following these steps, you can use Valgrind to check for memory leaks in your C code on macOS using macPort.