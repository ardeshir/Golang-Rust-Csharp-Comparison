use DBIish;  
use HTTP::UserAgent;  
use Text::CSV;  
  
sub print_table_api($db_conn_string) {  
    my $dbh = DBIish.connect($db_conn_string);  
    my $sth = $dbh.prepare('SELECT * FROM api;');  
    $sth.execute();  
    my @rows = $sth.fetchall();  
    my @columns = $sth.columns();  
  
    say "Table: api";  
    for @rows -> $row {  
        for $row.list -> $col {  
            say "{$columns[$++]}: $col";  
        }  
        say '';  
    }  
  
    $sth.finish();  
    $dbh.dispose();  
}  
  
sub process_csv_data($url, $db_conn_string) {  
    my $ua = HTTP::UserAgent.new();  
    my $response = $ua.get($url);  
    die $response.status_line() unless $response.is_success();  
    my $data = csv(in => $response.content.decode.split("\n")[1..*], headers => 0);  
  
    my $dbh = DBIish.connect($db_conn_string);  
    my $sth = $dbh.prepare(q:to/STATEMENT/);  
        CREATE TABLE IF NOT EXISTS api (  
            id SERIAL PRIMARY KEY,  
            url TEXT,  
            name TEXT,  
            created INTEGER  
        );  
    STATEMENT  
    $sth.execute();  
  
    for $data[1..*] -> $row {  
        $sth.execute($row[1], $row[2], $row[3]);  
    }  
  
    $dbh.commit();  
    $sth.finish();  
    $dbh.dispose();  
  
    say "Data inserted successfully!";  
}  
  
sub api_handler() {  
    my $db_conn_string = "dbname=data user=postgres password=postgres host=localhost port=5432 sslmode=disable";  
    my $dbh = DBIish.connect($db_conn_string);  
    my $sth = $dbh.prepare('SELECT * FROM api;');  
    $sth.execute();  
    my @rows = $sth.fetchall();  
    my @columns = $sth.columns();  
    my @apis = map { my %api; %api{@columns} = $_; %api; }, @rows;  
  
    $sth.finish();  
    $dbh.dispose();  
  
    return @apis.encode('json');  
}  
  
sub main($url = 'https://www.ardeshir.io/file.csv', Bool :$web) {  
    my $db_conn_string = "dbname=data user=postgres password=postgres host=localhost port=5432 sslmode=disable";  
  
    process_csv_data($url, $db_conn_string);  
    print_table_api($db_conn_string);  
  
    if $web {  
        use Cro::HTTP::Router;  
        my $router = Cro::HTTP::Router.new;  
        $router.get('/api', -> $) { api_handler() };  
        Cro::HTTP::Server.new(router => $router).start;  
    }  
} 