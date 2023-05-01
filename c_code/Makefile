CC = clang  
CFLAGS = -Wall -Wextra -pedantic -std=c11 -O2  -I/usr/include/postgresql/
LDFLAGS = -lcurl -lpq -ljansson  
TARGET = main  
  
all: $(TARGET)  
  
$(TARGET): main.c  
	$(CC) $(CFLAGS) $< -o $@ $(LDFLAGS) 
  
clean:  
	rm -f $(TARGET) 