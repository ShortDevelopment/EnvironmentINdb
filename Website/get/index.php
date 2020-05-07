<!DOCTYPE html>
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

				$sql = "SELECT * FROM Umwelt";
				$result = $conn->query($sql);
				$count = 0;

				if ($result->num_rows > 0) {
				$count++;
  				  // output data of each row
  				  while($row = $result->fetch_assoc()) {
  				     
					  $db = $row["DB"];
					  $lange = $row["Lange"];
					  $hohe = $row["Hohe"];
					  $zeit = $row["Name"];
					  $comm = $row["Kommentar"];
					echo "$lange|$hohe|$db|$zeit|$comm"."<br/>";
   				 }
				} else {
   					 echo "0 Ergebnisse";
				}
				$conn->close();


?>

