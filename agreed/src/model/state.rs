use crate::engine::Player;

#[derive(Debug)]
pub struct World {
	players: Vec<Player>,
}

impl Player {
	fn new(id: i64, name: String) -> Self {
		Player {
			x: 0.0,
			y: 0.0,
			id,
			name,
		}
	}
}

impl World {
	pub fn new() -> Self {
		World {
			players: Vec::new(),
		}
	}

	pub fn add_player(&mut self, player_name: String) -> i64 {
		let player_id = self.players.len() as i64;
		let player = Player::new(player_id, player_name);
		let id = player.id;
		self.players.push(player);
		id
	}

	pub fn update_player(&mut self, player_id: i64, delta_x: f32, delta_y: f32) -> Option<i64> {
		let player = self.get_player(player_id);
		match player {
			Some(p) => {
				p.x += delta_x;
				p.y += delta_y;
				Some(p.id)
			}
			None => None,
		}
	}

	pub fn get_players(&self) -> &Vec<Player> {
		&self.players
	}

	fn get_player(&mut self, player_id: i64) -> Option<&mut Player> {
		for p in &mut self.players {
			if p.id == player_id {
				return Some(p);
			}
		}
		None
	}
}
