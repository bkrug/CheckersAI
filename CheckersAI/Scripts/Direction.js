var Direction = function () {
    var self = this;

    self.Build = function (data, callback) {
        var directionNames = ["UP_LEFT", "UP_RIGHT", "DOWN_LEFT", "DOWN_RIGHT"];
        for (var i = 0; i < directionNames.length; ++i) {
            if (data[directionNames[i]] == null || data[directionNames[i]] == undefined)
                alert('There is no value for direction type ' + directionNames[i]);
            Direction[directionNames[i]] = data[directionNames[i]];
        }
        if (callback)
            callback();
    };

    Direction.UP_LEFT = null;
    Direction.UP_RIGHT = null;
    Direction.DOWN_LEFT = null;
    Direction.DOWN_RIGHT = null;
};