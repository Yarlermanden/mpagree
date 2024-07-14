use crate::validator::Player;

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

#[cfg(test)]
mod tests {
	use std::sync::Mutex;

	use super::*;

	#[test]
	pub fn test_update_player() {
		let world = Mutex::new(World::new());
		let player_id = world.lock().unwrap().add_player("test1".to_string());
		let (delta_x, delta_y) = (0.2, 0.5);
		let success = world.lock().unwrap().update_player(player_id, delta_x, delta_y);

		assert!(success.is_some());
		assert_eq!(success.unwrap(), player_id);
	}
}
