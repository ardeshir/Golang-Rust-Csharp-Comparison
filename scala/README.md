# Scala 

## To configure, build, and run the Scala code with scala-cli, follow these steps: 
1. Install Scala by following the instructions on their website: [https://www.scala-lang.org/download/]
2. Open a terminal or command prompt and navigate to the directory where your Scala file is located.
3. Compile the Scala file using the scalac command:
- scalac Main.scala
4. Run the compiled code using the scala command: 
- scala Main 

You should now see the output of the Scala program in your terminal.

## Setup macOS port or direct download with cs 
- sudo port install scala3.2
- curl -fL https://github.com/VirtusLab/coursier-m1/releases/latest/download/cs-aarch64-apple-darwin.gz --insecure | gzip -d > cs && chmod +x cs && (xattr -d com.apple.quarantine cs || true) && ./cs setup
- Check that JVM is installed, on macOS: 
- sudo port install openjdk11

## SBT 
- Open a command prompt or terminal window and navigate to the directory that contains the build.sbt file and the Main.scala file. 
- Run the command sbt assembly. This will compile the code and create an executable JAR file with all the library dependencies included. 
- Once the build process is complete, you can find the executable JAR file in the target/scala-2.12 directory (or a similar directory if you're using a different version of Scala). 
- To run the executable JAR file, navigate to the directory containing the JAR file and run the command java -jar <filename>.jar, replacing <filename> with the actual name of the JAR file. 