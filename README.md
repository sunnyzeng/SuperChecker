# SuperChecker

An application to check whether super disbursements have been paid according to a set of rules

<b>Projects</b>
<ul style=list-style-position:inside;">
    <li>SuperChecker.Core -   contains DTOs</li>
    <li>SuperChecker.Data -   contains helper class to parse excel data</li>
    <li>SuperChecker.Service   -   contains the main logic for the checking</li>
    <li>SuperChecker.Service.UnitTests   -   some tests for the main business logic + more</li>
    <li>SuperChecker   -   console app to use the ISuperDataReader & ISuperCheckerService to produce the results</li>
</ul>

<b>Approaches</b><br />
<ol style=list-style-position:inside;">
    <li>define the DTO used</li>
    <li>quick test using ExcelDataReader for input</li>
    <li>used TDD for the SuperCheckerService to refine the logic </li>
    <li>finalize the ExcelDataReader to map the input data required from the Excel</li>
    <li>wire up the console app</li>
    <li>sanity check the solution</li>
</ol>

<b>Features</b>
<ul style=list-style-position:inside;">
    <li>layered approach</li>
    <li>used a simple DI container from Microsoft</li>
    <li>unit tests project for the main logic</li>
</ul>

<b>How to run</b>
build and run SuperChecker

<b>Future enhancements</b>
<ul style=list-style-position:inside;">
    <li>review/refine the SuperDataReader, potentiall use 3rd party library to map the input data into entities</li>
    <li>more tests to cover boundaries and exceptions</li>
    <li>improve the display/formatting of the outputs</li>
</ul>