<%@page import="java.io.File"%>
<%@ page language="java" contentType="text/html; charset=US-ASCII"
    pageEncoding="US-ASCII"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=US-ASCII">
<title>Insert title here</title>

<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.2/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
<script src="http://code.jquery.com/jquery-latest.js"></script>   
<script src="easyXDM.min.js"></script>   
<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.2/jquery-ui.min.js"></script>
<script>
$(document).ready(function() {
	 
    //Stops the submit request
    $("#myAjaxRequestForm").submit(function(e){
           e.preventDefault();
    });
    
    //checks for the button click event
    $("#myButton").click(function(e){
           
            //get the form data and then serialize that
            dataString = $("#myAjaxRequestForm").serialize();
            
            //get the form data using another method 
            var filePath = $("input:checked+label").text(); 
            dataString = "filePath=" + filePath;
            
            //make the AJAX request, dataType is set to json
            //meaning we are expecting JSON data in response from the server
            $.ajax({
                type: "POST",
                url: "SomeServlet",
                data: dataString,
                dataType: "json",
                
                //if received a response from the server
                success: function( data, textStatus, jqXHR) {
                    //our country code was correct so we have some information to display
                          $("#ajaxResponse").html("");
                          $("#ajaxResponse").append("<b>Response:</b> " + data.response + "<br/>");
                         /*$("#ajaxResponse").append("<b>Country Code:</b> " + data.countryInfo.code + "<br/>");
                         $("#ajaxResponse").append("<b>Country Name:</b> " + data.countryInfo.name + "<br/>");
                         $("#ajaxResponse").append("<b>Continent:</b> " + data.countryInfo.continent + "<br/>");
                         $("#ajaxResponse").append("<b>Region:</b> " + data.countryInfo.region + "<br/>");
                         $("#ajaxResponse").append("<b>Life Expectancy:</b> " + data.countryInfo.lifeExpectancy + "<br/>");
                         $("#ajaxResponse").append("<b>GNP:</b> " + data.countryInfo.gnp + "<br/>"); */
                },
                
                //If there was no resonse from the server
                error: function(jqXHR, textStatus, errorThrown){
                     console.log("Something really bad happened " + textStatus);
                      $("#ajaxResponse").html(jqXHR.responseText);
                },
                
                //capture the request before it was sent to server
                beforeSend: function(jqXHR, settings){
                    //adding some Dummy data to the request
                    settings.data += "&dummyData=whatever";
                    //disable the button until we get the response
                    $('#myButton').attr("disabled", true);
                },
                
                //this is called after the response or error functions are finsihed
                //so that we can take some action
                complete: function(jqXHR, textStatus){
                    //enable the button 
                    $('#myButton').attr("disabled", false);
                }
      
            });        
    });
 
});
</script>
        
</head>
<body>
	<% File f = new File("/Users/Deliany/Desktop/text_files");
		  File[] list = f.listFiles();%>
	<form id="myAjaxRequestForm">
	<fieldset>
	<legend>Text files that can be analyzed</legend>
	<% for(File file : list) { 
		if(file.isHidden() == false) {
	%>
	<p>
	    <input type="radio" name="file" >
	    <label for="file" ><%= file.getAbsolutePath() %></label>
	  
	</p>
	  <%}} %>
	  <p>
         <input id="myButton" type="button" value="Analyze" />
     </p>
	   </fieldset>
	</form>
	
	
	<div id="anotherSection">
        <fieldset>
            <legend>Response from jQuery Ajax Request</legend>
                 <div id="ajaxResponse"></div>
        </fieldset>
    </div>   
</body>
</html>