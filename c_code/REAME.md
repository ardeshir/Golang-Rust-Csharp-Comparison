# C on macOS with MacPort
- sudo port install curl postgresql15 jansson 
- CFLAGS = -Wall -Wextra -pedantic -std=c11 -O2  -I/usr/include/postgresql/ -I/opt/local/include/postgresql15/ -I/opt/local/include/ 
- LDFLAGS = -L/opt/local/lib -ljansson -L/opt/local/lib/postgresql15 -lpq -lcurl 
- CFLAGS += -I/opt/local/include/postgresql13  
- CFLAGS += -I/opt/local/include/jansson  
- libcurl flags:
```--disable-shared --enable-static --prefix=/tmp/curl --disable-ldap --disable-sspi --without-librtmp --disable-ftp --disable-file --disable-dict --disable-telnet --disable-tftp --disable-rtsp --disable-pop3 --disable-imap --disable-smtp --disable-gopher --disable-smb --without-libidn --enable-ares
```

# C on linux Ubuntu 
1. To compile and run the code, you will need to follow the following steps: 
2. Install the required dependencies:
- libcurl: sudo apt-get install libcurl4-openssl-dev
- libpq: sudo apt-get install libpq-dev
- jansson: sudo apt-get install libjansson-dev 
- Create a Makefile with the following content: 
```
CC = clang  
CFLAGS = -Wall -Wextra -pedantic -std=c11 -O2  
LDFLAGS = -lcurl -lpq -ljansson  
TARGET = program  
  
all: $(TARGET)  
  
$(TARGET): main.c  
	$(CC) $(CFLAGS) $< -o $@ $(LDFLAGS)  
  
clean:  
	rm -f $(TARGET)  
```
3. Save the C code to a file named main.c. 
- Run make command to compile the code. 
4. Run the compiled program by typing ./program in the terminal. 
5. If you want to pass a URL to process the CSV data from, you can pass it as a command-line argument. 
- For example, ./main https://www.example.com/file.csv 
6. If you want to run the program as a web service, pass --web as a command-line argument. For example, ./main http://ardeshir.io/file.csv --web
