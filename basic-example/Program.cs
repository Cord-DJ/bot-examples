// Server connection
var cord = new CordClient();

// Login by email/password and wait until Ready is received
await cord.LoginAsync("example@examplecom", "password");


// List all joined rooms
foreach (var room in cord.Rooms) {
    Console.WriteLine($"Cord is ready {room.Id} name {room.Name}");
}

// List all available users
foreach (var user in cord.Users) {
    Console.WriteLine($"Cord is ready {user.Id} name {user.Name}");
}

// Enter room by URI link
var roomLink = "3ir5ug379kp";
await cord.EnterRoom(roomLink);


cord.MessageCreated += async (room, message) => {
    // If user is not member of the room anymore, his basic info is provided in <Author>
    var user = message.Member?.User ?? message.Author;
    var text = message.Text?.ToLower() ?? string.Empty;

    switch (text) {
        case "hey":
            await room.SendMessage($"Hello, {user.Name}!");
            break;
        case "how are you?":
            await room.SendMessage("I'm Fine, thanks! And you?");
            break;
        case "fine":
            await room.SendMessage("Cool");
            break;
        case "/clear all": {
            Console.WriteLine($"Deleting all messages {room.Messages?.Count}");

            foreach (var msg in room.Messages!) {
                await msg.Delete();
            }
            break;
        }
    }
    
    if (text.Contains("fuck")) {
        await message.Delete();
        await room.SendMessage("Don't swar!");
    }
};

// Get room by URI link
var currentRoom = cord.Rooms.First(x => x.Link == roomLink);

// Add BOT to the queue
try {
    await currentRoom.AddToQueue(cord.CurrentUser);
    Console.WriteLine("Added to queue");
} catch {
    Console.WriteLine("Not in queue");
}

// Wait 'till the world ends
await Task.Delay(-1);
