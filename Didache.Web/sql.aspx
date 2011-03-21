<%@ Page Language="C#" ClientIDMode="Static" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web.Services" %>
<%@ Import Namespace="System.Web.Script" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="System.Web.Script.Services" %>

<script runat="server">
static string baseConnection = @"data source=.\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|didache.mdf;User Instance=true;";
static bool multidatabase = false;
void Page_Load() {
	if (!IsPostBack) {
		if (multidatabase) {
			DatabasesList.DataSource = GetDatabases();
			DatabasesList.DataBind();
		}
	}
}

public string[] GetDatabases()
{
    // get databases
    SqlConnection sqlConnection = new SqlConnection(baseConnection);
    sqlConnection.Open();

    SqlCommand sqlCommand = new SqlCommand("sp_databases", sqlConnection);
    sqlCommand.CommandType = CommandType.StoredProcedure;

    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

    List<string> databases = new List<string>();

    while (sqlDataReader.Read())
    {
        databases.Add(sqlDataReader.GetString(0));
    }

    sqlDataReader.Close();
    sqlConnection.Close();

    databases.Sort();

    return databases.ToArray();
}


public string[] GetTables(string databaseName)
{
    string connectionString = baseConnection + ((multidatabase) ? "Initial Catalog=" + databaseName + ";" : "");

    // get databases
    SqlConnection sqlConnection = new SqlConnection(connectionString);
    sqlConnection.Open();
    SqlCommand sqlCommand = new SqlCommand("SELECT table_name FROM INFORMATION_SCHEMA.Tables WHERE TABLE_TYPE = 'BASE TABLE'", sqlConnection);

    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

    List<string> tables = new List<string>();

    while (sqlDataReader.Read())
    {
        tables.Add(sqlDataReader.GetString(0));
    }

    sqlDataReader.Close();
    sqlConnection.Close();

    tables.Sort();

    return tables.ToArray();
}
	
	

void GetTablesButton_Click(object sender, EventArgs e) {
	//DataOutput.DataSource = GetTables(DatabasesList.SelectedValue);
	//DataOutput.DataBind();
	
	TableNamesRepeater.DataSource = GetTables(DatabasesList.SelectedValue);
	TableNamesRepeater.DataBind();	
}

void RunQueryButton_Click(object sender, EventArgs e) {

	string databaseName = DatabasesList.SelectedValue;
	string sql = SqlQuery.Text;
	
	string connectionString = baseConnection + ((multidatabase) ? "Initial Catalog=" + databaseName + ";" : "");

	// get databases
	SqlConnection sqlConnection = new SqlConnection(connectionString);
	sqlConnection.Open();
	
	SqlCommand sqlCommand = null;
	SqlDataReader sqlDataReader = null;
	
	// check for "GO" statements
	Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
	string[] lines = regex.Split(sql);
	
	if (lines.Length > 1) {
	
		foreach (String line in lines) {
			//Response.Write("line...<br/>");
			//Response.Flush();

			try {
				if (!String.IsNullOrWhiteSpace(line)) {

					sqlCommand = new SqlCommand(line, sqlConnection);
					sqlCommand.ExecuteNonQuery();
				}
			} catch {
				//Response.Write("error...<br/>");
				//Response.Flush();
			}	
		}
	
	} else {		
		sqlCommand = new SqlCommand(sql, sqlConnection);
		sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.KeyInfo);	
		
		DataTable schemaTable = sqlDataReader.GetSchemaTable();

		if (schemaTable == null || schemaTable.Rows == null || schemaTable.Rows.Count == 0)
			return;

		TableName.Value = schemaTable.Rows[0]["BaseTableName"].ToString();

		// create table
		/*
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("<table>");

		// header
		sb.AppendLine("<thead>");
		sb.AppendLine("<tr>");
		foreach (DataColumn column in schemaTable.Columns)
		{
			sb.AppendLine("<td>" + column.ColumnName + "</td>");
		}
		sb.AppendLine("</tr>");
		sb.AppendLine("</thead>");

		sb.AppendLine("<tbody>");
		foreach (DataRow row in schemaTable.Rows) {
			sb.AppendLine("<tr>");
					
			foreach (DataColumn column in schemaTable.Columns) {
				sb.AppendLine("<td class=\"" + column.DataType.ToString() + "\">" + row[column].ToString() + "</td>");
			}

			sb.AppendLine("</tr>");
		}
		sb.AppendLine("</tbody>");
		sb.AppendLine("</table>");
		 * */

		StringBuilder sb = new StringBuilder();

		
		
		sb.AppendLine("<table id=\"data-table\">");

		// header
		sb.AppendLine("<thead>");
		sb.AppendLine("<tr>");
		foreach (DataRow row in schemaTable.Rows) {	
			
			//if (!(bool)row["IsKey"]) {
			sb.AppendLine("<th" + (((bool)row["IsKey"]) ? " class=\"key\"" : "") + ">" + row["ColumnName"].ToString() + "</th>");	
			//}									
			
		}
		sb.AppendLine("<td><input type=\"button\" class=\"addrow\" value=\"add row\" /></td>");	
		sb.AppendLine("</tr>");
		sb.AppendLine("</thead>");

		sb.AppendLine("<tbody>");
		while (sqlDataReader.Read()) {
			// assemble key info
			List<string> keyValueArray = new List<string>();
			List<string> keyNameArray = new List<string>();
			foreach (DataRow row in schemaTable.Rows) {
				if ((bool)row["IsKey"]) {
					keyNameArray.Add(row["ColumnName"].ToString());
					keyValueArray.Add(sqlDataReader[row["ColumnName"].ToString()].ToString());
				}
			}
			string keyValues = string.Join("|", keyValueArray.ToArray());
			string keyNames = string.Join("|", keyNameArray.ToArray());

			sb.AppendLine("<tr data-keynames=\"" + keyNames + "\" data-keyvalues=\"" + keyValues + "\">");

			foreach (DataRow row in schemaTable.Rows) {
				string columnName = row["ColumnName"].ToString();
				string type = row["DataType"].ToString();
				string value = sqlDataReader[row["ColumnName"].ToString()].ToString();
				string html = "";			
				
				if ((bool)row["IsKey"]) {
					sb.AppendLine("<th data-columnname=\"" + columnName + "\">" + value + "</th>");
				} else {


					switch (type) {
						case "System.Boolean":
							html = "<input data-columnname=\"" + columnName + "\" data-type=\"" + type + "\" type=\"checkbox\" " + ((value.ToLower() == "true") ? " checked=\"checked\"" : "") + " />";
							break;
						default:
						case "System.String":
							html = "<input data-columnname=\"" + columnName + "\" data-type=\"" + type + "\" type=\"text\" value=\"" + value + "\" />";
							break;
						case "System.DateTime":
							html = "<input data-columnname=\"" + columnName + "\" data-type=\"" + type + "\" type=\"text\" class=\"date\" value=\"" + value + "\" />";
							break;						
					}
					
					sb.AppendLine("<td>" + html + "</td>");
				}
			}
			sb.AppendLine("<th><input type=\"button\" class=\"update\" value=\"save\" /><input type=\"button\" class=\"delete\" value=\"del\" /></th>");
			sb.AppendLine("</tr>");
		}
		sb.AppendLine("</tbody>");
		sb.AppendLine("</table>");
		

		TableOutput.Text = sb.ToString();
		
		
		sqlDataReader.Close();		
	}	
	

	
	sqlConnection.Close();

}

[WebMethod()]
[ScriptMethod(ResponseFormat = ResponseFormat.Json, XmlSerializeString = false)] 
public static object InsertSql(string databaseName, string tableName, Dictionary<String, String> columns)
{
	// updates
	List<string> columnNames = new List<string>();
	List<string> columnValues = new List<string>();
	foreach (string columnName in columns.Keys)
	{
		var value = columns[columnName];
		if (value != null)
		{
			columnNames.Add(columnName);
			columnValues.Add("'" + value.Replace("'", "''") + "'");
		}
	}

	string sql = "INSERT INTO " + tableName + " (" + String.Join(", ", columnNames.ToArray()) + ") VALUES (" + String.Join(", ", columnValues.ToArray()) + "); SELECT SCOPE_IDENTITY();";

	string connectionString = baseConnection + ((multidatabase) ? "Initial Catalog=" + databaseName + ";" : "");

	// get databases
	SqlConnection sqlConnection = new SqlConnection(connectionString);
	sqlConnection.Open();
	SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);

	string message = "";
	
	try {
		object value = sqlCommand.ExecuteScalar();
		
		return new {
			value = value,
			error = ""
		};
	
		//message = @"{""value"":""" + value.ToString().Replace(@"""", @"""""") + @"""}";
	}
	catch (Exception e)
	{
		return new
		{
			value = "",
			error = e.ToString()
		};
		//message = @"{""values"":[], ""sql"":""" + sql.Replace(@"""", @"""""") + @""",""error"": """ + e.ToString() + @"""}";
	}

	//return message;
}


[WebMethod()]
public static string UpdateSql(string databaseName, string tableName, string keyNames, string keyValues, Dictionary<String, String> columns)
{
	// updates
	List<string> updates = new List<string>();
	foreach (string columnName in columns.Keys) {
		updates.Add(columnName + " = '" + columns[columnName].Replace("'", "''") + "'");	
	}
	// keys
	List<string> keys = new List<string>();
	string[] keyValuesArray = keyValues.Split(new char[] { '|' });
	string[] keyNamesArray = keyNames.Split(new char[] { '|' });
	for (int i = 0; i < keyNamesArray.Length; i++)
	{
		keys.Add(keyNamesArray[i] + " = '" + keyValuesArray[i].Replace("'","''") + "'");
	}

	string sql = "UPDATE " + tableName + " SET " + String.Join(", ", updates.ToArray()) + " WHERE " + String.Join(" AND ", keys.ToArray());
	
	string connectionString = baseConnection + ((multidatabase) ? "Initial Catalog=" + databaseName + ";" : "");

	// get databases
	SqlConnection sqlConnection = new SqlConnection(connectionString);
	sqlConnection.Open();
	SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
	
	string message = "true";


	//message = sql;
	
	try {
		sqlCommand.ExecuteNonQuery();
	} catch (Exception e) {
		message = @"{values:[], error: """ + e.ToString() + @"""}";
	}

	return message;
}

[WebMethod()]
public static string DeleteSql(string databaseName, string tableName, string keyNames, string keyValues)
{

	// keys
	List<string> keys = new List<string>();
	string[] keyValuesArray = keyValues.Split(new char[] { '|' });
	string[] keyNamesArray = keyNames.Split(new char[] { '|' });
	for (int i = 0; i < keyNamesArray.Length; i++)
	{
		keys.Add(keyNamesArray[i] + " = '" + keyValuesArray[i].Replace("'", "''") + "'");
	}

	string sql = "DELETE FROM " + tableName + " WHERE " + String.Join(" AND ", keys.ToArray());

	string connectionString = baseConnection + ((multidatabase) ? "Initial Catalog=" + databaseName + ";" : "");

	// get databases
	SqlConnection sqlConnection = new SqlConnection(connectionString);
	sqlConnection.Open();
	SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);

	string message = "";
	try
	{
		sqlCommand.ExecuteNonQuery();
	}
	catch (Exception e)
	{
		message = @"{error: """ + e.ToString() + @"""}";
	}

	return message;
}   
</script>
<!DOCTYPE html>
<html>
<head>
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />

	<title>SQL Utility</title>
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script> 
	<script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/jquery-ui.min.js"></script> 
	<link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.7.0/themes/base/jquery-ui.css" rel="stylesheet" /> 

	<style>
	#datatable {
		border: solid 1px #666;
		border-collapse: collapse;
	}
	#datatable td, #datatable th {
		border: solid 1px #666;
		border-collapse: collapse;
	}	
	</style>
</head>
<body>
<form id="Form1" runat="server">
	<h1>SQL Utility</h1>

	<div>
		<asp:dropdownlist id="DatabasesList" runat="Server" />
		<asp:button id="GetTablesButton" onclick="GetTablesButton_Click" runat="Server" Text="Get Tables" />
		<br />
		<asp:TextBox ID="SqlQuery" runat="server" TextMode="MultiLine" width="600" Height="200" />
		<br />
		<asp:button id="RunQueryButton" onclick="RunQueryButton_Click" runat="Server" Text="Run Query" />
	</div>
	<div>
	ALTER TABLE {TABLENAME} 
	ADD {COLUMNNAME} {TYPE} {NULL|NOT NULL} 
	CONSTRAINT {CONSTRAINT_NAME} DEFAULT {DEFAULT_VALUE}
	</div>
	<div>
		<asp:literal ID="TableOutput" runat="server" EnableViewState="false" />
		<asp:HiddenField ID="TableName" runat="server" EnableViewState="false" />
	</div>
	<div>
		<asp:Repeater id="TableNamesRepeater" runat="serveR" EnableViewState="false">
			<headertemplate>
			<table id="table-list">
			</headertemplate>

			<ItemTemplate>
			<tr>
				<td><%# Container.DataItem.ToString() %></td>
				<td><span class="open" rel="<%# Container.DataItem.ToString() %>">Open</a></td>			
			</tr>
			</ItemTemplate>
			
			<footertemplate>
			</table>
			</footertemplate>		
		</asp:Repeater>
		
		<asp:GridView ID="DataOutput" runat="server" EnableViewState="false" />
	</div>


<script>
	$('#table-list span.open').click(function () {
		var tableName = $(this).attr('rel');

		$('#SqlQuery').val('SELECT TOP 20 * FROM ' + tableName + ';');
		$('#RunQueryButton').trigger('click');

	});


	// date pickers
	/*
	var i = document.createElement("input");
	i.setAttribute("type", "date");
	if (i.type == "text") {
	$('input[type=datetime]').datepicker();
	}
	*/
	$('input.date').datepicker();


	function getValues(tr) {
		var colValues = {};

		// find all the values from the
		tr.find('td input').each(function () {
			var input = $(this),
			columnName = input.attr('data-columnname'),
			datatype = input.attr('data-type'),
			value = input.val();

			// type checkeing
			if (input.attr('type') == 'checkbox')
				value = (input[0].checked);

			if (datatype == 'System.Int32')
				value = parseInt(value, 10);

			if (datatype == 'System.Double')
				value = parseFloat(value);

			// add to changes
			colValues[columnName] = value;
		});

		return colValues;
	}

	$('#data-table input').change(function () {
		$(this).data('changed', true);
	});

	$('#data-table input.addrow').click(function () {
		var table = $(this).closest('table'),
		newRow = table.find('tbody tr').first().clone();

		// clear out values
		newRow
		.attr('data-keyvalues', '')
		.find('td input')
			.val('')
		.end()
		.find('td input[type=checkbox]')
			.attr('checked', '')
		.end()
		.find('th[data-columnname]')
			.html('')
		.end()
		.find('input.delete, input.update')
			.hide()
		.end()
		.find('th').last()
			.append($('<input type="button" value="insert" class="insert" />'));

		newRow
		.prependTo(table.find('tbody'));
	});


	$('#data-table').delegate('input.insert', 'click', function () {
		var 
		tr = $(this).closest('tr'),
		changes = getValues(tr);

		// send new rowint
		$.ajax({
			url: document.location.pathname + '/InsertSql',
			type: "POST",
			dataType: 'json',
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify({ "tableName": $('#TableName').val(), "databaseName": $('#DatabasesList').val(), "columns": changes }),
			success: function (data) {
				console.log('success', data, data.value, data.d.value);

				//var data = JSON.parse

				/* TODO MUTLPLE KEYS
				var keys = tr.attr('data-keynames').split('-');
				for (var key in keys) {
				tr.find('th.' + key).html(d.values[key]);
				}		
				*/

				// show new key
				tr
				.attr('data-keyvalues', data.d.value);
				tr
				.find('th[data-columnname=' + tr.attr('data-keynames') + ']').html(data.d.value);

				// buttons, highlight
				tr
				.find('input.insert')
					.hide()
				.end()
				.find('input.update, input.delete')
					.show()
				.end()
				.effect('highlight');
			},
			error: function (e) {
				console.log('typical', e);
			}
		});
	});


	$('#data-table').delegate('input.update', 'click', function () {
		var 
		tr = $(this).closest('tr'),
		keyValues = tr.attr('data-keyvalues'),
		keyNames = tr.attr('data-keynames'),
		changes = getValues(tr);

		// send AJAX changes!
		$.ajax({
			url: document.location.pathname + '/UpdateSql',
			type: "POST",
			dataType: 'json',
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify({ "tableName": $('#TableName').val(), "databaseName": $('#DatabasesList').val(), "keyValues": keyValues, "keyNames": keyNames, "columns": changes }),
			success: function (d) {
				console.log('success', d);
				tr.effect('highlight');
			},
			error: function (e) {
				console.log('typical', e);
			}
		});
	});


	$('#data-table').delegate('input.delete', 'click', function () {
		var 
		tr = $(this).closest('tr'),
		keyValues = tr.attr('data-keyvalues'),
		keyNames = tr.attr('data-keynames');

		if (confirm('Are you 100% sure you want to delete this row: ' + keyNames + ' = ' + keyValues)) {
			// send AJAX changes!
			$.ajax({
				url: document.location.pathname + '/DeleteSql',
				type: "POST",
				dataType: 'json',
				contentType: "application/json; charset=utf-8",
				data: JSON.stringify({ "tableName": $('#TableName').val(), "databaseName": $('#DatabasesList').val(), "keyValues": keyValues, "keyNames": keyNames }),
				success: function (d) {
					console.log('success', d);
					tr.effect('highlight');
					tr.remove();
				},
				error: function (e) {
					console.log('typical', e);
				}
			});
		}
	});



</script>
</form>
</body>
</html>