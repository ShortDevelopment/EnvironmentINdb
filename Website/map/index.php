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
?>
<script>
		function Schreiben(n, w) {
    var a = new Date();

    a = new Date(a.getTime() + 1000 * 3600 * 24 * 365);

    document.cookie = n + '=' + w + '; expires=' + a.toGMTString() + ';';
}

// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

function Lesen(n) {
    a = document.cookie;
    res = '';

    while (a != '') {
        while (a.substr(0, 1) == ' ') {
            a = a.substr(1, a.length);
        }

        cookiename = a.substring(0, a.indexOf('='));

        if (a.indexOf(';') != -1) {
            cookiewert = a.substring(a.indexOf('=') + 1, a.indexOf(';'));
        }
        else {
            cookiewert = a.substr(a.indexOf('=') + 1, a.length);
        }

        if (n == cookiename) {
            res = cookiewert;
        }

        i = a.indexOf(';') + 1;

        if (i == 0) {
            i = a.length
        }

        a = a.substring(i, a.length);
    }

    return (res)
}

// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

function Loeschen(n) {
    document.cookie = n + '=; expires=Thu, 01-Jan-70 00:00:01 GMT;';
}
		function change(a){
			
			if(a.checked==true){
				Schreiben(a.getAttribute("name"), '0');
				location.reload();
			}else{
				Schreiben(a.getAttribute("name"), '1');
				location.reload();
			}
		}
	
		function getCookie(name) {
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1) {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    }
    else
    {
        begin += 2;
        var end = document.cookie.indexOf(";", begin);
        if (end == -1) {
        end = dc.length;
        }
    }
    // because unescape has been deprecated, replaced with decodeURI
    //return unescape(dc.substring(begin + prefix.length, end));
    return decodeURI(dc.substring(begin + prefix.length, end));
} 
		
		function start(){
			if (getCookie('a')==null) {
				Schreiben('a', '0');
				document.getElementById('1').checked = true;
			}
			if (getCookie('b')==null) {
				Schreiben('b', '0');
				document.getElementById('2').checked = true;
			}
			if (getCookie('c')==null) {
				Schreiben('c', '0');
				document.getElementById('3').checked = true;
			}
			if (getCookie('d')==null) {
				Schreiben('d', '0');
				document.getElementById('4').checked = true;
			}
		}
	</script>
<html lang="de" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>Karte</title>
    <link href="/../css/Main.css" rel="stylesheet" />    
    <link href="/../Media/favicon.ico" rel="icon"/>
    <link href="/../Media/favicon.ico" rel="shortcut icon"/>
    <link href="/../Media/favicon.ico" rel="apple-touch-icon"/>   

	<script>
		var delayInMilliseconds = 1000; //1 second

setTimeout(function() {
  start();
}, delayInMilliseconds);
</script>
    <script>
        //Variablen, die "Global" benötigt werden
        var pinInfobox;
        var trafficManager;
        var contextMenu;
		

        /*
        //"Animiert" das "Boom"-Bild. Eigentliche animation per css
        setInterval(function () {
            if (document.getElementById("explosion").style.height == "100px") {
                document.getElementById("explosion").style.height = "150px";
            } else {
                document.getElementById("explosion").style.height = "100px";
            }
        }, 700);
        */
        // "customMapStyle" *1 => Keine Beschriftung
        var NoLabels = {
            "version": "1.0",
            "elements": {
                "mapElement": {
                    "labelVisible": false
                }
            }
        }        
        function GetMap() {
            //Laden der Karte
             map = new Microsoft.Maps.Map('#myMap', {
                 showDashboard: false                 
             });
            //Kartenoptionen
             var loc1 = new Microsoft.Maps.Location(48.888304, 9.130323);
             Microsoft.Maps.Events.addHandler(map, 'viewchangestart', function () { document.getElementById('loader').style.display = 'none'; document.getElementById('NavBar').style.display = 'block'; });
             map.setView({ center: loc1, zoom: 11, labelOverlay: Microsoft.Maps.LabelOverlay.hidden });
             map.setOptions({ customMapStyle: NoLabels }); // *1

            //Modul für die Verkehrslage
             Microsoft.Maps.loadModule('Microsoft.Maps.Traffic', function () {                
                 trafficManager = new Microsoft.Maps.Traffic.TrafficManager(map);                 
                 //trafficManager.show();
             });
             Microsoft.Maps.Events.addHandler(map, 'rightclick', function (e) {
                 contextMenu.setOptions({
                     location: e.location,
                     visible: true
                 });
             });

			
			
			<?php
				$sql = "SELECT * FROM Umwelt";
				$result = $conn->query($sql);
				$count = 0;

				if ($result->num_rows > 0) {
				
  				  // output data of each row
  				  while($row = $result->fetch_assoc()) {
  				  	  $count++;
					  $db = $row["DB"];
					  $lange = $row["Lange"];
					  $hohe = $row["Hohe"];
					  $zeit = $row["Name"];
					  $comment = $row["Kommentar"];
					  
					  $colorr=0;
					  $colorg=0;
					  $colorb=0;
					  
					  if($db<60) {
						  $colorg=255;
					  }
					  if($db>=60){
						  $colorr=255;
							  $colorg=147;
						  
						  
					  }
					  if($db>=80){
						  $colorr=255;
						   $colorg=0;
					  }
					  
					  
					  if($hohe==0){
						$hohe=1;  
					  }
					  if($lange==0){
						$lange=1;  
					  }
					  if($lange==100){
						$lange=1;  
					  }
					  if($hohe==100){
						$hohe=1;  
					  }
					  if($comment != null){
					  	$zeit = $zeit . '\n Kommentar: ' . $comment;
					  }
					  
					  
					   
					  $i=20;
					  $teile = explode(" ", $zeit);
					  $a = $teile[4];
					  $b = explode(":", $a);
					  $i = $b[0];
					  
					   if($i >= 0){
						  if($i < 6){
						  		if($_COOKIE['a']=='1'){
									continue;
								}
						  }
				  	  }
					  if($i >= 6){
						  if($i < 12){
						  	if($_COOKIE['b']=='1'){
									continue;
								}
						  }
				  	  }
					  if($i >= 12){
						  if($i < 18){
						  	if($_COOKIE['c']=='1'){
									continue;
								}
						  }
				  	  }
					  if($i >= 18){
						  if($i < 24){
						  	if($_COOKIE['d']=='1'){
									continue;
								}
						  }
				  	  }

					 					  
					  
					 
					  echo "
					  var center$count = new Microsoft.Maps.Location($hohe, $lange);
					  var pin$count = new Microsoft.Maps.Pushpin(center$count, { color: 'rgb($colorr,$colorg,$colorb)',size:5,text:'$db', typeName: 'TypeBad' });			 
           			  pin$count.Title = '$db db';
					  
             		  pin$count.Description = 'Gemessen am $zeit';          
           			  map.entities.push(pin$count);
           			  Microsoft.Maps.Events.addHandler(pin$count, 'click', displayInfobox);";
   				 }
				} else {
   					 echo "0 results";
				}
				
			?>
			
			
			
            //Definierung der Infobox, die angezeigt wird, wenn der Pushpin angeklickt wird
             contextMenu = new Microsoft.Maps.Infobox(map.getCenter(), {
                 htmlContent: '<div class="ContextMenu"><input type="button" value="Heran zoomen" onclick="map.setView({zoom: map.getZoom() + 1});closeContextMenu();"/><input type="button" value="Heraus zoomen" onclick="map.setView({zoom: map.getZoom() - 1});closeContextMenu();"/><hr/><input type="button" value="KPFiMa-Medien" onclick="" disabled/></div>',
                 visible: false
             });
             contextMenu.setMap(map);
             document.body.onmousedown = function () {
                 closeContextMenu();
             };

            //Definierung des Pushpins (Markierung auf der Karte)
          //   var pin1 = new Microsoft.Maps.Pushpin(loc1, { color: 'rgb(255,0,0)',size:5,text:'2', typeName: 'TypeBad' });
       //     pin1.Title = "Titel";
        //     pin1.Description = "Test";          
         //    map.entities.push(pin1);
         //    Microsoft.Maps.Events.addHandler(pin1, 'click', displayInfobox);
			 


            //Definierung einer 2. Infobox als ContextMenü
             pinInfobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false });
             map.entities.push(pinInfobox);
         }
         function closeContextMenu() {
             contextMenu.setOptions({ visible: false });
         }
         function displayInfobox(e) {
             pinInfobox.setOptions({ title: e.target.Title, description: e.target.Description, visible: true, offset: new Microsoft.Maps.Point(0, 25) });
             pinInfobox.setLocation(e.target.getLocation());
             showInfos(e);
         }
         function showInfos(e) {
             document.getElementById("title").innerText = e.target.Title;
             document.getElementById("infos").innerText = e.target.Description;
             document.getElementById("db").innerText = e.target.getText() + "db";
         }
         function ToogleTraffic(ele){
             if (ele.innerText == "Verkehrslage einblenden") {
                 ele.innerText = "Verkehrslage ausblenden"
                 trafficManager.show();
                 document.getElementById("NavBar").style.left = '110px';
             } else {
                 ele.innerText = "Verkehrslage einblenden"
                 trafficManager.hide();
                 document.getElementById("NavBar").style.left = '10px';
             }
         }
         function ToogleLabels(ele) {
             if (ele.innerText == "Beschriftung einblenden") {
                 ele.innerText = "Beschriftung ausblenden"                 
                 map.setOptions({ customMapStyle: {} });
             } else {
                 ele.innerText = "Beschriftung einblenden"                 
                 map.setOptions({ customMapStyle: NoLabels });
             }             
         }
    </script>    
</head>
<body>
    <!-- Header-Menü -->
    <div id="header">
        <a href="../index.html" style="top:50%; left:15%;"><img src="/../Media/house.png" />Home</a>
        <img class="img" id="explosion" src="/../Media/explosion.png" style="top:10%; right:10%;"/>
        <img class="img" id="car" src="/../Media/car.png"/>        
        <div class="text">
            Auch Lärm belastet die Umwelt!
        </div>
    </div>
    <!-- Container für Karte und co -->
    <div id="MapContainer">     
        <!-- Karte -->         
        <div id="myMap"></div> 
        <!-- Buttons zur "Navigation" -->
        <div id="NavBar">
            <button onclick="ToogleTraffic(this)">Verkehrslage einblenden</button>
            <button onclick="ToogleLabels(this)">Beschriftung einblenden</button>
        </div>
        <!-- Ladeanimation -->
        <div id="loader" enabled></div>  
    </div>  
    <!-- Weiterer Inhalt -->
    <div id="Main">
        <div id="Content">
            <style>
                #Content p{
                    padding:0px;
                    margin-top:5px;
                    margin-bottom:5px;
                    display:inline-block;
                }
                #InfoContainer{

                }                
                #InfoContainer h3, #InfoContainer infos{                    
                    margin-top:0px;
                    margin-bottom:5px;
                }
            </style>
            Ja, <div class="text">Auch Lärm belastet die Umwelt!</div>
            <br />
            <p>Wähle links einen Punkt aus, um die Lautstärke zu sehen</p>
            <br />
            <p><?php

$sql1 = "SELECT * FROM Umwelt";
$result1 = $conn->query($sql1);
    
$count1=0;
while($row1 = $result1->fetch_assoc()) {
  				  	  $count1++;
}

echo "Eingetragene Punkte: " . $count1;
?> </p>
            <br />
			<h4>Filter</h4>
			<div>
  <input type="checkbox" id="1" name="a"
         onchange="change(this);">
  <label for="scales">0-6 Uhr</label>
</div>
			<div>
  <input type="checkbox" id="2" name="b" onchange="change(this);">
         
  <label for="scales">6-12 Uhr</label>
</div>
			<div>
  <input type="checkbox" id="3" name="c" onchange="change(this);">
         
  <label for="scales">12-18 Uhr</label>
</div>
			<div>
  <input type="checkbox" id="4" name="d" onchange="change(this);">
         
  <label for="scales">18-24 Uhr</label>
				<script>
			if(Lesen('a')=='0'){
				
				document.getElementById('1').checked = true;
			}else{
				
				document.getElementById('1').checked = false;
			}
			if(Lesen('b')=='0'){
				document.getElementById('2').checked = true;
			}else{
				document.getElementById('2').checked = false;
			}
			if(Lesen('c')=='0'){
				document.getElementById('3').checked = true;
			}else{
				document.getElementById('3').checked = false;
			}
			if(Lesen('d')=='0'){
				document.getElementById('4').checked = true;
			}else{
				document.getElementById('4').checked = false;
			}</script>
</div>
            <br />
            <div id="InfoContainer">
                <h3 id="title"></h3>
                <b id="db"></b>
                <p id="infos"></p>
            </div>            
        </div>
    </div>
    <!-- Bing Maps script -->
    <script type='text/javascript' src='https://www.bing.com/api/maps/mapcontrol?callback=GetMap&key=AmJaFwCkQefP9Gw3fcUAkCutEpGr9wlcJMk6fpumhzWtyfMaVz9g9WACrHmh7JK6' defer></script>  
	
</body>
</html>