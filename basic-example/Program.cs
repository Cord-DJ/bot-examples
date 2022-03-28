// Server connection
var cord = new CordClient(Endpoint.Sigma);

var tcsReady = new TaskCompletionSource();
cord.Ready += async () => tcsReady.SetResult();

// Login by email password
await cord.LoginAsync("example@examplecom", "password");

// Wait 'till client is ready
await tcsReady.Task;


cord.Ready += async () => {
    // All joined rooms
    foreach (var room in cord.Rooms) {
        Console.WriteLine($"Cord is ready {room.Id} name {room.Name}");
    }

    // All available users
    foreach (var user in cord.Users) {
        Console.WriteLine($"Cord is ready {user.Id} name {user.Name}");
    }

    // Enter room by URI link
    await cord.EnterRoom("3ir5ug379kp");
};

cord.MessageCreated += async (room, message) => {
    // If user is not member of the room anymore, his basic info is provided in <Author>
    var user = message.Member?.User ?? message.Author;
    var text = message.Text?.ToLower() ?? string.Empty;

    if (text == "hey") {
        await room.SendMessage($"Hello, {user.Name}!");
    } else if (text == "how are you?") {
        await room.SendMessage("I'm Fine, thanks! And you?");
    } else if (text == "fine") {
        await room.SendMessage("Cool");
    }

    if (text.Contains("fuck")) {
        await message.Delete();
        await room.SendMessage("Don't swar!");
    }

    if (text == "/clear all") {
        Console.WriteLine($"Deleting all messages {room.Messages?.Count}");

        foreach (var msg in room.Messages!) {
            await msg.Delete();
        }
    }
};

// Wait 'till the world ends
await Task.Delay(-1);
