var BoardBuilder = function () {
    var self = this;

    self.Build = function(board) {
        var results = '';
        for (var r = 0; r < 8; ++r) {
            results += '<div class="js-row ui-row">';
            for (var c = 0; c < 8; ++c) {
                var pieceClass = '';
                switch (board.PieceLayout[r][c]) {
                    case Piece.UP_TEAM:
                        pieceClass = 'ui-up';
                        break;
                    case Piece.UP_TEAM_KING:
                        pieceClass = 'ui-up-king';
                        break;
                    case Piece.DOWN_TEAM:
                        pieceClass = 'ui-down';
                        break;
                    case Piece.DOWN_TEAM_KING:
                        pieceClass = 'ui-down-king';
                        break;
                }
                var playableClass = (r % 2 != c % 2) ? 'js-playable ui-playable' : 'ui-nonplayable';
                var positionClass = 'r' + r + 'c' + c;
                results += '<div class="js-square ui-square ' + positionClass + ' ' + playableClass + ' ' + pieceClass + '"></div>';
            }
            results += '</div>';
        }
        return results;
    }
};