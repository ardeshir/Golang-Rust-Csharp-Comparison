CC = clang -v 
CFLAGS = -Wall -Wextra -pedantic -std=c11 -O2  -I/usr/include/postgresql/ -I/opt/local/include/postgresql15/ -I/opt/local/include/ 
LDFLAGS = -L/opt/local/lib -ljansson -L/opt/local/lib/postgresql15 -lpq -lcurl 
TARGET = main  
  
all: $(TARGET)  
  
$(TARGET): main.c  
	$(CC) $(CFLAGS) $< -o $@ $(LDFLAGS) 
  
clean:  
	rm -f $(TARGET) 

