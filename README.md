This is a learning project in which I developed a fully featured meeting management system. 
The application includes user registration,role-based authorization, file uploads, and an Entity Framework Core-based database.

## Key Features

- Users can create meetings and add participants.
- Only the meeting organizer can update or delete a meeting.
- The Admin role has a dedicated controller for role management.
- File uploads and storage are implemented following security best practices.
- Meetings can include images and PDF attachments.
- Clear role-based access control.

## Endpoint Access Permissions

- **Login, Register** — accessible to everyone.
- **CreateMeeting, GetMeetings\*, GetMeeting** — accessible to authenticated users.
- **UpdateMeeting, DeleteMeeting, CreateParticipant, RemoveParticipant** — organizer only.
- **UploadMeetingFile, UploadAttachment** — organizer only.
- **Download** — accessible to everyone (access should ideally be restricted to participants).

## Technologies

- ASP.NET Core
- Entity Framework Core
- Microsoft SQL Server
- ASP.NET Core Identity
- JWT Authentication
- File Validation
- Swagger



=====Suomeksi======


Tämä on oppimisprojekti, jossa toteutin täysiverisen tapaamisten hallintajärjestelmän.
Sovelluksessa on käyttäjien rekisteröinti, roolit, autorisointi, tiedostojen lataus sekä
EF Core -pohjainen tietokanta.

## Keskeiset ominaisuudet
- Käyttäjät voivat luoda tapaamisia ja lisätä niihin osallistujia.
- Vain tapaamisen järjestäjä voi muokata tai poistaa tapaamisen.
- Admin-roolilla on erillinen controller roolien hallintaan.
- Tiedostojen lataus ja tallennus toteutettu turvallisuusohjeiden mukaisesti.
- Tapaamisiin voi liittää kuvia ja PDF-tiedostoja.
- Selkeä roolipohjainen pääsynhallinta.

## Endpointien käyttöoikeudet
- **Login**, **Register** – kaikille.
- **CreateMeeting**, **GetMeetings***, **GetMeeting** – kirjautuneille.
- **UpdateMeeting**, **DeleteMeeting**, **CreateParticipant**, **RemoveParticipant** – vain järjestäjälle.
- **UploadMeetingFile**, **UploadAttachment** – vain järjestäjälle.
- **Download** – kaikille (suositeltavaa rajata osallistujille).

## Teknologiat
- ASP.NET Core  
- Entity Framework Core  
- MS SQL  
- Identity  
- JWT  
- File Validation
- Swagger

