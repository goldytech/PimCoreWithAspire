var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddContainer("redis", "redis:alpine")
    .WithArgs("redis-server", "--maxmemory", "128mb", "--maxmemory-policy", "volatile-lru", "--save", "\"\"")
    .WithVolume("pimcore-demo-redis-data", "/data");

// Mariadb
var db = builder.AddContainer("db", "mariadb:10.11")
    .WithArgs("mysqld", "--character-set-server=utf8mb4", "--collation-server=utf8mb4_general_ci",
        "--innodb-file-per-table=1", "--log-bin=mysql-bin")
    .WithVolume("pimcore-demo-database", "/var/lib/mysql")
    .WithEnvironment("MYSQL_ROOT_PASSWORD", "ROOT")
    .WithEnvironment("MYSQL_DATABASE", "pimcore")
    .WithEnvironment("MYSQL_USER", "pimcore")
    .WithEnvironment("MYSQL_PASSWORD", "pimcore");

var php = builder.AddContainer("php", "pimcore/pimcore:php8.3-debug-latest")
    .WithEnvironment("COMPOSER_HOME", "/var/www/html")
    .WithEnvironment("PHP_IDE_CONFIG", "serverName=localhost")
    .WithBindMount(Path.GetFullPath("."), "/var/www/html")
    .WithVolume("pimcore-demo-tmp-storage", "/tmp")
    //.WithContainerUser("851801118:851800513")
    .WaitFor(db);
var nginx = builder.AddContainer("nginx", "nginx:stable-alpine")
    .WithEndpoint(targetPort:8081, port: 80, name: "web")
    .WithBindMount(Path.GetFullPath("."), "/var/www/html", isReadOnly: true)
    .WithBindMount(Path.GetFullPath("nginx.conf"), "/etc/nginx/conf.d/default.conf", isReadOnly: true)
    .WithBindMount("/etc/nginx/conf.d", "/etc/nginx/conf.d", isReadOnly: false)
    .WaitFor(php);

builder.Build().Run();
