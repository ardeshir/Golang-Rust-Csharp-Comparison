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