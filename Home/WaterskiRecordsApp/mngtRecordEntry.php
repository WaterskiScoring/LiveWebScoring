<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start();
include_once( "showRecordsInit.php" );
?>

<!DOCTYPE html>
<html>

<head>
<?php
include( "PageHeaderDef.php" );
?>
</head>

<body>

<div data-role="page" data-theme="a" id="mngtRecordEntry"	>

	<div data-role="header">
		<h1>
		Water Ski Records For Eastern Region
		<br/>Manage Record Entry<br/>
		</h1>
	</div><!-- /header -->

    <div class="ui-content" role="main">
		<form method="post" name="Record Entry" action="updateRecordEntry.php" id="RecordEntryForm" data-ajax="true">
		    <div class="ui-field-contain" style="max-width: 500px; padding: 2px;">
		    <div class="ui-field-contain">
				<label for="SkierName">SkierName:</label>
				<input name="SkierName" id="SkierName" data-mini="true" value="" placeholder="Text input" type="text" maxlength="128" />
			</div>
		    <div class="ui-field-contain">
				<label for="MemberId">MemberId:</label>
				<input name="MemberId" id="MemberId" data-mini="true" value="" placeholder="Text input" type="text" maxlength="9" />
			</div>
		    <div class="ui-field-contain">
				<label for="Division">Division:</label>
				<select name="Division" id="Division" data-mini="true" data-inline="true">
				<?php
					$SqlCommand = "Select ListCode, CodeValue from CodeValueList "
						. "Where ListName = 'AWSAAgeGroup' Order by SortSeq";
					$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
					$DataRow = mysql_num_rows($SqlResult);
					if ( $DataRow != 0 ) {
						while ($DataRow = mysql_fetch_assoc($SqlResult)) {
							echo "<option value='" . $DataRow['ListCode'] . "'>" . $DataRow['CodeValue'] . "</option>";
						}
					}
					mysql_free_result($SqlResult);
				?>
  				</select>
			</div>
		    <div class="ui-field-contain">
				<label for="Federation">Federation:</label>
				<select name="Federation" id="Federation" data-mini="true" data-inline="true">
				<?php
					$SqlCommand = "Select ListCode, CodeValue from CodeValueList "
						. "Where ListName = 'Federation' Order by SortSeq";
					$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
					$DataRow = mysql_num_rows($SqlResult);
					if ( $DataRow != 0 ) {
						while ($DataRow = mysql_fetch_assoc($SqlResult)) {
							echo "<option value='" . $DataRow['ListCode'] . "'>" . $DataRow['CodeValue'] . "</option>";
						}
					}
					mysql_free_result($SqlResult);
				?>
  				</select>
			</div>
		    <div class="ui-field-contain">
				<label for="Region">Region:</label>
				<select name="Region" id="Region" data-mini="true" data-inline="true">
				<?php
					$SqlCommand = "Select Distinct CodeValue from CodeValueList "
						. "Where ListName = 'StateRegion' Order by SortSeq, CodeValue";
					$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
					$DataRow = mysql_num_rows($SqlResult);
					if ( $DataRow != 0 ) {
						while ($DataRow = mysql_fetch_assoc($SqlResult)) {
							echo "<option value='" . $DataRow['ListCode'] . "'>" . $DataRow['CodeValue'] . "</option>";
						}
					}
					mysql_free_result($SqlResult);
				?>
  				</select>
			</div>
		    <div class="ui-field-contain">
				<label for="State">State:</label>
				<select name="State" id="State" data-mini="true" data-inline="true">
				<?php
					$SqlCommand = "Select ListCode, CodeValue from CodeValueList "
						. "Where ListName = 'StateRegion' AND CodeValue = 'E - East' Order by SortSeq, CodeValue, ListCode";
					$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
					$DataRow = mysql_num_rows($SqlResult);
					if ( $DataRow != 0 ) {
						while ($DataRow = mysql_fetch_assoc($SqlResult)) {
							echo "<option value='" . $DataRow['ListCode'] . "'>" . $DataRow['ListCode'] . "</option>";
						}
					}
					mysql_free_result($SqlResult);
				?>
  				</select>
			</div>
		    <div class="ui-field-contain">
				<label for="Club">Club:</label>
				<input name="Club" id="Club" data-mini="true" value="" type="text" maxlength="256" />
			</div>
		    <div class="ui-field-contain">
				<label for="SanctionId">MemberId:</label>
				<input name="SanctionId" id="SanctionId" data-mini="true" value="" type="text" maxlength="6" />
			</div>
		    <div class="ui-field-contain">
				<label for="TourName">TourName:</label>
				<input name="TourName" id="TourName" data-mini="true" value="" type="text" maxlength="128" />
			</div>
		    <div class="ui-field-contain">
				<label for="EventClass">EventClass:</label>
				<select name="EventClass" id="EventClass" data-mini="true" data-inline="true">
				<?php
					$SqlCommand = "Select ListCode, CodeValue from CodeValueList "
						. "Where ListName = 'Class' Order by SortSeq";
					$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
					$DataRow = mysql_num_rows($SqlResult);
					if ( $DataRow != 0 ) {
						while ($DataRow = mysql_fetch_assoc($SqlResult)) {
							echo "<option value='" . $DataRow['ListCode'] . "'>" . $DataRow['CodeValue'] . "</option>";
						}
					}
					mysql_free_result($SqlResult);
				?>
  				</select>
			</div>
		    <div class="ui-field-contain">
				<label for="SiteFK">Site:</label>
				<select name="SiteFK" id="SiteFK" data-mini="true" data-inline="true">
				<?php
					$SqlCommand = "Select PK, SiteName from Site Order by SiteName";
					$SqlResult = mysql_query($SqlCommand) or die (mysql_error());
					$DataRow = mysql_num_rows($SqlResult);
					if ( $DataRow != 0 ) {
						while ($DataRow = mysql_fetch_assoc($SqlResult)) {
							echo "<option value='" . $DataRow['PK'] . "'>" . $DataRow['SiteName'] . "</option>";
						}
					}
					mysql_free_result($SqlResult);
				?>
  				</select>
			</div>

		    <div class="ui-field-contain">
				<label for="Event" class="select">Event:</label>
				<select name="Event" id="Event" data-mini="true" data-inline="true">
					<option value="Slalom">Slalom</option>
					<option value="Trick">Trick</option>
					<option value="Jump">Jump</option>
					<option value="Overall">Overall</option>
  				</select>
			</div>

		    <div class="ui-field-contain">
				<label for="RecordType" class="select">RecordType:</label>
				<select name="RecordType" id="RecordType" data-mini="true" data-inline="true">
					<option value="Region">Region</option>
					<option value="State">State</option>
					<option value="Club">Club</option>
					<option value="Site">Site</option>
  				</select>
			</div>

		    <div class="ui-field-contain">
				<fieldset data-role="controlgroup" data-type="horizontal" data-mini="true">
					<legend>RecordStatus:</legend>
					<input name="RecordStatus" id="RecordStatus-Active" value="A" checked="checked" type="radio" />
					<label for="RecordStatus-Active">Active</label>
					<input name="RecordStatus" id="RecordStatus-Inactive" value="I" type="radio" />
					<label for="RecordStatus-Inactive">Inactive</label>
					<input name="RecordStatus" id="RecordStatus-Rejected" value="R" type="radio" />
					<label for="RecordStatus-Rejected">Rejected</label>
				</fieldset>
			</div>

		    <div class="ui-field-contain">
				<label for="RecordDate">RecordDate:</label>
				<input name="RecordDate" id="RecordDate" data-mini="true" value="" type="date" maxlength="10" />
			</div>

		    <div class="ui-field-contain">
				<label for="Notes">Notes:</label>
				<textarea  cols="64" rows="3 name="Notes" id="Notes"></textarea>

			</div>

			<div data-role="controlgroup" data-type="horizontal" data-mini="true">
				<input value="Retrieve" data-icon="refresh" type="button">
				<input value="Insert" data-icon="plus" type="button">
				<input value="Update" data-icon="edit" type="button">
				<input value="Delete" data-icon="delete" type="button">
				<input value="Reset" data-icon="recycle" type="reset">
			</div>
			</div>

		</form>
	</div><!-- /content -->
<?php
include( "PageFooter.php" );
?>

</div><!-- /page -->

</body>
</html>
<?php
include_once( "showRecordsTerm.php" );
?>
