
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

