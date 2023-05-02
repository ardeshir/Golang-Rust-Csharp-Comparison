import java.net.URL  
import java.sql.{Connection, DriverManager}  
import java.time.{Instant, format}  
import java.util.Properties  
  
import com.sun.net.httpserver.{HttpExchange, HttpHandler, HttpServer}  
import _root_.io.circe._  
import _root_.io.circe.generic.auto._  
import _root_.io.circe.syntax._  

  
import _root_.scala.collection.mutable.ListBuffer  
import _root_.scala.io.Source  
  
object Main {  
  case class Api(id: Int, url: String, name: String, created: String)  
  
  def main(args: Array[String]): Unit = {  
    val url = "https://www.ardeshir.io/file.csv"  
    val web = false  
    Class.forName("org.postgresql.Driver")
    val dbProps = new Properties()  
    dbProps.setProperty("user", "postgres")  
    dbProps.setProperty("password", "postgres")  
    dbProps.setProperty("sslmode", "disable")  
  
    val dbConnString = "jdbc:postgresql://localhost:5432/data"  
    val dbConnection = DriverManager.getConnection(dbConnString, dbProps)  
  
    try {  
      processCSVData(url, dbConnection)  
      printTableApi(dbConnection)  
    } finally {  
      dbConnection.close()  
    }  
  
    if (web) {  
      startWebServer(dbConnection)  
    }  
  }  
  
  def processCSVData(url: String, dbConnection: Connection): Unit = {  
    val csvData = downloadCSV(url)  
    createApiTable(dbConnection)  
  
    val insertQuery = "INSERT INTO api (url, name, created) VALUES (?, ?, ?)"  
    val insertStmt = dbConnection.prepareStatement(insertQuery)  
  
    val dateFormatter = format.DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")  
  
    for (i <- 1 until csvData.size) {  
      val url = csvData(i)(1)  
      val name = csvData(i)(2) 
      val created = csvData(i)(3)  
  
      insertStmt.setString(1, url)  
  
      insertStmt.setString(2, name)  
      insertStmt.setString(3, created)  
  
      insertStmt.executeUpdate()  
    }  
  
    println("Data inserted successfully!")  
  }  
  
  def downloadCSV(urlString: String): ListBuffer[Array[String]] = {  
    val url = new URL(urlString)  
    val connection = url.openConnection()  
  
    val csvData = ListBuffer[Array[String]]()  
  
    try {  
      val inputStream = connection.getInputStream  
      val source = Source.fromInputStream(inputStream)  
      val lines = source.getLines()  
  
      while (lines.hasNext) {  
        csvData += lines.next().split(",").map(_.trim)  
      }  
  
      source.close()  
    } catch {  
      case e: Exception => e.printStackTrace()  
    }  
  
    csvData  
  }  
  
  def createApiTable(dbConnection: Connection): Unit = {  
    val createTableQuery =  
      """    
        |CREATE TABLE IF NOT EXISTS api (    
        |  id SERIAL PRIMARY KEY,    
        |  url TEXT,    
        |  name TEXT,    
        |  created TEXT     
        |)    
        |""".stripMargin  
  
    val statement = dbConnection.createStatement()  
    statement.execute(createTableQuery)  
    statement.close()  
  }  
  
  def printTableApi(dbConnection: Connection): Unit = {  
    val selectQuery = "SELECT * FROM api"  
    val statement = dbConnection.createStatement()  
    val resultSet = statement.executeQuery(selectQuery)  
  
    val apiList = ListBuffer[Api]()  
  
    while (resultSet.next()) {  
      val id = resultSet.getInt("id")  
      val url = resultSet.getString("url")  
      val name = resultSet.getString("name")  
      val created = resultSet.getString("created") 
  
      val api = Api(id, url, name, created)  
      apiList += api  
    }  
  
    resultSet.close()  
    statement.close()  
  
    println("Table: api")  
    for (api <- apiList) {  
      println(s"id: ${api.id}, url: ${api.url}, name: ${api.name}, created: ${api.created}")  
    }  
  }  
  
  def startWebServer(dbConnection: Connection): Unit = {  
    val server = HttpServer.create(new java.net.InetSocketAddress("localhost", 8080), 0)  
    server.createContext("/api", new ApiHandler(dbConnection))  
    server.setExecutor(null)  
    server.start()  
  }  
  
  class ApiHandler(dbConnection: Connection) extends HttpHandler {  
    implicit val instantEncoder: Encoder[Instant] =  
      Encoder.encodeString.contramap[Instant](i => i.toString)  
  
    def handle(httpExchange: HttpExchange): Unit = {  
      val selectQuery = "SELECT * FROM api"  
      val statement = dbConnection.createStatement()  
      val resultSet = statement.executeQuery(selectQuery)  
  
      val apiList = ListBuffer[Api]()  
  
      while (resultSet.next()) {  
        val id = resultSet.getInt("id")  
        val url = resultSet.getString("url")  
        val name = resultSet.getString("name")  
        val created = resultSet.getString("created")  
  
        val api = Api(id, url, name, created)  
        apiList += api  
      }  
  
      resultSet.close()  
      statement.close()  
  
      // val response = Json.encodeToString(apiList)  
      val response = Printer.noSpaces.copy(dropNullValues = true).print(apiList.asJson)
      httpExchange.getResponseHeaders.set("Content-Type", "application/json")  
      httpExchange.sendResponseHeaders(200, response.getBytes.length)  
      val outputStream = httpExchange.getResponseBody  
      outputStream.write(response.getBytes)  
      outputStream.close()  
    }  
  }  
}  


/* 
import java.net.URL  
import java.sql.{Connection, DriverManager}  
import java.time.Instant  
import java.time.format.DateTimeFormatter  
import java.util.Properties  
  
import scala.collection.mutable.ListBuffer  
import scala.io.Source  
  
object Main {  
  case class Api(id: Int, url: String, name: String, created: Instant)  
  
  def main(args: Array[String]): Unit = {  
    val url = "https://www.ardeshir.io/file.csv"  
    val web = false  
  
    val dbProps = new Properties()  
    dbProps.setProperty("user", "postgres")  
    dbProps.setProperty("password", "postgres")  
    dbProps.setProperty("sslmode", "disable")  
  
    val dbConnString = "jdbc:postgresql://localhost:5432/data"  
    val dbConnection = DriverManager.getConnection(dbConnString, dbProps)  
  
    try {  
      processCSVData(url, dbConnection)  
      printTableApi(dbConnection)  
    } finally {  
      dbConnection.close()  
    }  
  
    if (web) {  
      startWebServer(dbConnection)  
    }  
  }  
  
  def processCSVData(url: String, dbConnection: Connection): Unit = {  
    val csvData = downloadCSV(url)  
    createApiTable(dbConnection)  
  
    val insertQuery = "INSERT INTO api (url, name, created) VALUES (?, ?, ?)"  
    val insertStmt = dbConnection.prepareStatement(insertQuery)  
  
    val dateFormatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")  
  
    for (i <- 1 until csvData.size) {  
      val url = csvData(i)(1)  
      val name = csvData(i)(2)  
      val created = Instant.parse(csvData(i)(3), dateFormatter)  
  
      insertStmt.setString(1, url)  
      insertStmt.setString(2, name)  
      insertStmt.setTimestamp(3, java.sql.Timestamp.from(created))  
  
      insertStmt.executeUpdate()  
    }  
  
    println("Data inserted successfully!")  
  }  
  
  def downloadCSV(urlString: String): ListBuffer[Array[String]] = {  
    val url = new URL(urlString)  
    val connection = url.openConnection()  
  
    val csvData = ListBuffer[Array[String]]()  
  
    try {  
      val inputStream = connection.getInputStream  
      val source = Source.fromInputStream(inputStream)  
      val lines = source.getLines()  
  
      while (lines.hasNext) {  
        csvData += lines.next().split(",").map(_.trim)  
      }  
  
      source.close()  
    } catch {  
      case e: Exception => e.printStackTrace()  
    }  
  
    csvData  
  }  
  
  def createApiTable(dbConnection: Connection): Unit = {  
    val createTableQuery =  
      """  
        |CREATE TABLE IF NOT EXISTS api (  
        |  id SERIAL PRIMARY KEY,  
        |  url TEXT,  
        |  name TEXT,  
        |  created TIMESTAMP  
        |)  
        |""".stripMargin  
  
    val statement = dbConnection.createStatement()  
    statement.execute(createTableQuery)  
    statement.close()  
  }  
  
  def printTableApi(dbConnection: Connection): Unit = {  
    val selectQuery = "SELECT * FROM api"  
    val statement = dbConnection.createStatement()  
    val resultSet = statement.executeQuery(selectQuery)  
  
    val apiList = ListBuffer[Api]()  
  
    while (resultSet.next()) {  
      val id = resultSet.getInt("id")  
      val url = resultSet.getString("url")  
      val name = resultSet.getString("name")  
      val created = resultSet.getTimestamp("created").toInstant  
  
      val api = Api(id, url, name, created)  
      apiList += api  
    }  
  
    resultSet.close()  
    statement.close()  
  
    println("Table: api")  
    for (api <- apiList) {  
      println(s"id: ${api.id}, url: ${api.url}, name: ${api.name}, created: ${api.created}")  
    }  
  }  
  
  def startWebServer(dbConnection: Connection): Unit = {  
    val server = HttpServer.create(new InetSocketAddress("localhost", 8080), 0)  
    server.createContext("/api", new ApiHandler(dbConnection))  
    server.setExecutor(null)  
    server.start()  
  }  
  
  class ApiHandler(dbConnection: Connection) extends HttpHandler {  
    implicit val instantEncoder: Encoder[Instant] =  
      Encoder.encodeString.contramap[Instant](i => i.toString)  
  
    def handle(httpExchange: HttpExchange): Unit = {  
      val selectQuery = "SELECT * FROM api"  
      val statement = dbConnection.createStatement()  
      val resultSet = statement.executeQuery(selectQuery)  
  
      val apiList = ListBuffer[Api]()  
  
      while (resultSet.next()) {  
        val id = resultSet.getInt("id")  
        val url = resultSet.getString("url")  
        val name = resultSet.getString("name")  
        val created = resultSet.getTimestamp("created").toInstant  
  
        val api = Api(id, url, name, created)  
        apiList += api  
      }  
  
      resultSet.close()  
      statement.close()  
  
      val response = apiList.asJson.noSpaces  
      httpExchange.getResponseHeaders.set("Content-Type", "application/json")  
      httpExchange.sendResponseHeaders(200, response.getBytes.length)  
      val outputStream = httpExchange.getResponseBody  
      outputStream.write(response.getBytes)  
      outputStream.close()  
    }  
  }  
}  
*/