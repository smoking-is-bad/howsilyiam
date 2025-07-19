This file contains instructions for updating the company.xml file.

The company.xml file defines the list of companies that will be made available to the
user during commissioning.  There is no means to add or remove companies via the SmartPims
application interface.  Such changes are only made by editing the content of the 
company.xml located at C:\ProgramData\SensorNetworks.  A default company.xml file will be
copied to the directory if no file exists when the SmartPims application is installed.  
If a company.xml already exists, it will not be updated when the SmartPims application is 
updated.

-- Editing Companies

List all the companies you want to make available to the user during commissioning.

** NOTE:  Any company listed here must have been previously created on the server via the
superuser administration panel.  The company must be set to an Active state and the ID
number must match the ID as shown in the Company List from the administration panel.

Use the following format to list multiple companies:

  <companies>
    <company>
      <id>Company A ID number</id>
      <name>Company A</name>
    </company>
    <company>
      <id>Company B ID number</id>
      <name>Company B</name>
    </company>
  </companies>


-- Editing  Sites, Plants, and Assets

List all the Sites, Plants, and Assets you want associated with each Company ID. Multiple
options for each category can be listed using the following format.

  <autofillplaces>
    <companyPlaces companyId="Company A ID number" companyName="Company A ID number">
      <places>
        <place type="site">Site A1</place>
        <place type="plant">Plant A1</place>
        <place type="plant">Plant A2</place>
        <place type="asset">Asset A1</place>
        <place type="asset">Asset A2</place>
        <place type="asset">Asset A3</place>
      </places>
    </companyPlaces>
    <companyPlaces companyId="Company B ID number" companyName="Company B ID number">
      <places>
        <place type="site">Site  B1</place>
        <place type="plant">Plant B1</place>
        <place type="asset">Asset B1</place>
        <place type="asset">Asset B2</place>
      </places>
    </companyPlaces>
  <auotfillplaces>
    
** NOTE:  If you add a new company to the top section of the company.xml file you must add
at least an empty <places> placeholder to the <autofillplaces> section that looks like the 
following. The actual places can be created later via the SmartPims application.

	<companyPlaces companyId="A" companyName="Company A">
      <places>
      </places>
    </companyPlaces>

    