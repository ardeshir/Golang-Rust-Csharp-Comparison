scalaVersion := "3.2.2"  
  
libraryDependencies ++= Seq(  
  "org.postgresql" % "postgresql" % "42.2.23",  
  "io.circe" %% "circe-core" % "0.14.1",  
  "io.circe" %% "circe-generic" % "0.14.1"  
)  



/* libraryDependencies ++= Seq(  
  "org.postgresql" % "postgresql" % "42.3.1",  
  "io.circe" %% "circe-core" % "0.14.1",  
  "io.circe" %% "circe-generic" % "0.14.1",  
  "com.sun.net.httpserver" % "http" % "20070405"  
)  */