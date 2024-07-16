extends Node

enum colors {BLACK, WHITE}
class_name __
# x,y

#we are missing the three special interactions in chess
#castling, promotion, en passant.


static func is_out_of_bounds(coords: Vector2): 
	if (coords.x < 0 || coords.y > 7 ||
	coords.x > 7 || coords.y < 0 ):
		return true
	return false

static func vector_to_chess_notation(vector: Vector2):
	var _column := ["a","b","c","d","e","f","g","h"]
	return _column[vector.x]+String(vector.y+1)

static func is_odd(num: int):
	return ((num%2) == 1)
	
static func follow_direction(board, origin: Vector2, color: int, direction: Vector2, result := []):

	var nextSquareCoord = Vector2(origin.x + direction.x, origin.y + direction.y)
	#if we reached the end of the board then we just return
	if (is_out_of_bounds(nextSquareCoord)):
		return result
		
	var nextSquare = board[nextSquareCoord.x][nextSquareCoord.y]

	#if there is a friendly piece we stop the path there
	#if it is enemy we add the square to the list and return 
	if nextSquare is Piece:
		if nextSquare.color != color: 
			result.append(nextSquare.coords)
		return result
	#if it's an empty square, we add it to the list and keep going
	elif not nextSquare:
		result.append(nextSquareCoord)
		return follow_direction(board, nextSquareCoord, color, direction, result)
	else: 
		printerr("what the fuck?", nextSquare)
		return -1

static func find_paths(board: Array, color: int, origin: Vector2, type: int):
	var directions = {
	1: Vector2(-1,-1),
	2: Vector2(0, -1),
	3: Vector2(1, -1),
	4: Vector2(-1, 0),
	6: Vector2(1,0),
	7: Vector2(-1,1),
	8: Vector2(0,1),
	9: Vector2(1,1),
	}
	var type_directions = {
	Piece.types.BISHOP: [7,9,3,1],
	Piece.types.ROOK: [8,2,4,6],
	Piece.types.QUEEN: [1,2,3,4,6,7,8,9]
	}
	
	var result := []
	for directionIndex in type_directions[type]:
		result.append_array(
			follow_direction(board, origin, color, directions[directionIndex])
			)
	return result

static func horse_moves(_board: Array, color: int, coords: Vector2) -> Array:
	var result := []
	var possibleMoves = [
		# up
		Vector2(coords.x - 1, coords.y + 2), 
		Vector2(coords.x + 1, coords.y + 2),
		# right
		Vector2(coords.x + 2, coords.y + 1),
		Vector2(coords.x + 2, coords.y - 1),
		# down
		Vector2(coords.x - 1, coords.y - 2),
		Vector2(coords.x + 1, coords.y - 2),
		# left
		Vector2(coords.x - 2, coords.y + 1),
		Vector2(coords.x - 2, coords.y - 1),
	]
	for coord in possibleMoves: 
		if (is_out_of_bounds(coord)):
			continue
		var piece = _board[coord.x][coord.y]
		if (piece && piece.color == color):
			continue			
		result.append(coord)
	return result

static func pawn_moves(_board: Array, color: int, coords: Vector2) -> Array:

	var result := []
	var y_move: int
	match(color):
		colors.WHITE:
			y_move =  1
		colors.BLACK: 
			y_move = -1

	var nextSquareCoord = Vector2(coords.x, coords.y + y_move) # 8
	var nextSquare = _board[nextSquareCoord.x][nextSquareCoord.y]
	
	var doubleStepSquareCoord = Vector2(coords.x, coords.y + (y_move*2)) #88
	var doubleStepSquare = _board[doubleStepSquareCoord.x][doubleStepSquareCoord.y]

	var diagonals = {
		0 :  [
			Vector2(coords.x - 1, coords.y - 1), #1
			Vector2(coords.x + 1 , coords.y - 1) # 3
		],
		1 :  [
			Vector2(coords.x + 1, coords.y + 1), # 9
			Vector2(coords.x - 1, coords.y + 1)  # 7
		]
	}	

	#checking if nextSquare is empty
	if (not nextSquare):
		result.append(nextSquareCoord)
	
	#double step rules
	if 	(coords.y == 1 && color == colors.WHITE 
	|| coords.y == 6 && color == colors.BLACK):
		if (not doubleStepSquare):
				result.append(doubleStepSquareCoord)

	#accounting for chomping
	for coord in diagonals[color]:
		if is_out_of_bounds(coord):
			continue
		var diagSquare = _board[coord.x][coord.y]
		if (diagSquare && 
			diagSquare.color != color):
			result.append(coord)
	
	return result

static func king_moves(_board: Array, color: int, coords: Vector2) -> Array:
	var directions = {
	1: Vector2(-1,-1),
	2: Vector2(0, -1),
	3: Vector2(1, -1),
	4: Vector2(-1, 0),
	6: Vector2(1,0),
	7: Vector2(-1,1),
	8: Vector2(0,1),
	9: Vector2(1,1),
	}
	var result := []
	for index in directions:
		var direction = directions[index]
		var potentialMove = Vector2(coords.x + direction.x, coords.y + direction.y)
		if (not is_out_of_bounds(potentialMove)):
			var square = _board[potentialMove.x][potentialMove.y]
			if (square is Piece && square.color == color):
					continue
			result.append(potentialMove)
	return result


static func pawn_check(board: Array, color: int, coords: Vector2): 
	var threats = []
	match color:
		colors.BLACK:
			threats.append(Vector2(coords.x + 1, coords.y - 1)) #3
			threats.append(Vector2(coords.x - 1, coords.y - 1)) #1
		colors.WHITE:
			threats.append(Vector2(coords.x + 1, coords.y + 1)) # 9
			threats.append(Vector2(coords.x - 1, coords.y + 1)) # 7
	for threat in threats:
		if (not is_out_of_bounds(threat)):
			var piece = board[threat.x][threat.y]
			if (piece && piece.type == 0 && piece.color == 1 - color):
				return true
	return false

static func is_king_in_check(board: Array, color: int):
	# 1 - color flips the color
	var opponent =  1 - color
	var threat_squares = []
	var king

	for row in board:
		for square in row:
			if square is Piece and square.color == opponent and square.type > 0:
				threat_squares += square.available_moves(board, false)
			elif square is Piece and square.color == color and square.type == 5:
				king = square
	if pawn_check(board, color, king.coords):
		return true
	for threat in threat_squares:
		if (threat == king.coords):
			return true
	return false
