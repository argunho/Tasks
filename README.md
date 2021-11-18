# Studieuppgift: Get things do (C#, Asp.Net Core 3.1, JavaScript, Razor, MVC, RestApi)

### Uppgifts projekten består av två projekt som ligger i samma i solution. Projekt som heter Tasks är en backend (webapi) samt koppling till
databasen och relation med databasen sker i denna projekten. TasksUI är en vanlig MVC projekt och körs i mitt fall som klientgränssnitt
och använder referenser from Tasks. Att logga in och registrera på hemsidan, var använd vanlig Identity och körs from TasksUI. Som hjälp att kommunicera mellan klientgränssnitt och web api, skapat en WebApiConnect class där med användning HttpClient, skapad en base link.

## På vilket sätt sparas uppgifterna i databasen?
### -Data från klientgränssnitt genom Ajax skickas först till mvc ActionsController därifrån skickas vidare till webapi projekt till rätt
endpoint.. Varsin method i ActionController avsett att kommunicera med rätt endpoint.

## Vilken säkerhetslösning är implementerad i API:et
#### - JWT security är implementerad att ingen oberoende kunde använda projekten. All endpoint kräver att dem blir anropat bara av inloggade användare. HandleTasks- och HandleUsersController är markerad i början med [Authorize] med detta förbjuden åtkomst till dem som är inte inloggad. I klass Help Repository skapas security token vid inloggning och registrering och sparas i projekt Loc anrop?
#### -Varje endpoint i webapi har sitt eget namn och vid varje anrop kontrolleras Id på inloggad användare om den användare som gjorde anrop är inte Author eller inte bjuden till uppgiftslista då nekas anrop.

## WebApi HandleTasksController - ansvarar för alla anrop som gäller
## Uppgifter och tags, kort förklaring till dem, ser du nedan:
#### ● [HttpGet] - hämtar och plockar all uppgifter var inloggad är author..
#### ● [HttpGet("Member")] - alla uppgifter var användare är inbjuden.
#### ● [HttpGet("GetTask/{id}")] - hämtas en uppgift by id.
#### ● [HttpGet("Tags/{id}")] - hämtas alla tags som tillhör till den uppgiften som har Id matchande till {id}
#### ● [HttpGet("SearchTasks/{key}")] - tar emot nyckelord {key} och letar bland alla dem Uppgifter och Tags, som tillhör till inloggad användare eller var användare är inbjudenhar, efter matchande ord (i namn, beskrivning och text) till nyckelord.
#### ● [HttpPost("NewTask")] - Avsett att ta emot data för nya uppgift och
skicka till db.
#### ● [HttpPost("NewTag")] - Avsett att ta emot för nya uppgiftens tag och skicka data till db.
#### ● [HttpPut("UpdateTask/{id}")] - Avsett att ta emot uppdaterad uppgiftens data och skicka vidare till db.
#### ● [HttpPut("Task/{id}/Member/{user_id}")] - att lägga till en annan användare till uppgift (inbjuda användare).
#### ● [HttpPut("TaskDone/{taskId}")] - uppdatera uppgiftens status till som färdig
#### ● [HttpPut("TagDone/{tagId}")] - uppdatera uppgiftens tags status till som är färdig
#### ● [HttpDelete("DeleteTask/{id}")] - radera uppgift och alla tillhörande tags
