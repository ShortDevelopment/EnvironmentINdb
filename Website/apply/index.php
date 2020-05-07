<?php
$servername = ":3306";
$username = "test";
$password = "";
$dbname = "umwelt";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
} 
echo "Connected successfully";


if($_POST["Latitude"]==null){
	exit;
}
$hohe = $_POST["Latitude"];
$lange = $_POST["Longitude"];
$db = $_POST["DB"];
$comment = $_POST["Kommentar"];


	
date_default_timezone_set('Europe/Berlin');
$daynames = ["Montag","Dienstag","Mittwoch","Donnerstag","Freitag", "Samstag", "Sonntag"];
$dayName = $daynames[date("N") - 1];
$date = date("d.m.Y");
$time = date("H:i");
$name = $dayName.", den ".$date." um ".$time." Uhr";

if($comment==null){
	$comment = '';
}

$sql = "INSERT INTO `Umwelt`(`DB`, `Lange`, `Hohe`, `Name`, `Kommentar`) VALUES ($db,$lange,$hohe,'$name','$comment')";



if ($conn->query($sql) === TRUE) {
    
} else {
    echo "Error: " . $sql . "<br>" . $conn->error;
}
?>

Hochladen Erfolgreich

