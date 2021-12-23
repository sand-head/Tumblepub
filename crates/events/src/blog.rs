use eventually::optional::Aggregate;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Blog {
  #[serde(skip_serializing)]
  pub id: i64,
  // Foreign blogs do not have users
  #[serde(skip_serializing)]
  pub user_id: Option<i64>,
  pub uri: Option<String>,
  pub name: String,
  pub domain: Option<String>,
  pub is_primary: bool,
  pub state: BlogState,
  pub private_key: Option<Vec<u8>>,
  pub public_key: Vec<u8>,
  pub created_at: DateTime<Utc>,
  pub updated_at: DateTime<Utc>,
}

#[derive(Debug, Clone, Copy, Serialize, Deserialize)]
#[serde(tag = "state")]
pub enum BlogState {
  Active {
    pub title: Option<String>,
    pub description: Option<String>,
    pub is_public: bool,
    pub is_nsfw: bool,
    pub theme_id: Option<Uuid>,
  },
  Tombstone,
}

#[derive(Debug)]
pub enum BlogCommand {
  Create,
  Discover,
  Update,
  Delete,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(tag = "type", content = "data")]
pub enum BlogEvent {
  Created {
    user_id: i64,
    uri: Option<String>,
    name: String,
    domain: Option<String>,
    is_primary: bool,
    state: BlogState,
    private_key: Option<Vec<u8>>,
    public_key: Vec<u8>,
    at: DateTime<Utc>,
  },
  Discovered {},
  Updated,
  Deleted,
}

#[derive(Debug, Clone, PartialEq, Eq, thiserror::Error)]
pub enum BlogError {
  #[error("Blog has not been created")]
  NotYetCreated,
  #[error("Blog with name `{0}` already exists")]
  AlreadyExists(String),
}

#[derive(Debug, Clone, Copy)]
pub struct BlogAggregate;

#[async_trait]
impl Aggregate for BlogAggregate {
  type Id = i64;
  type State = Blog;
  type Event = BlogEvent;
  type Command = BlogCommand;
  type Error = BlogError;

  fn apply_first(event: Self::Event) -> Result<Self::State, Self::Error> {
    match event {
      BlogEvent::Created {
        user_id,
        uri,
        name,
        domain,
        is_primary,
        state,
        private_key,
        public_key,
        at,
      } => Ok(Blog {
        id: 0,
        user_id,
        uri,
        name,
        domain,
        is_primary,
        state,
        private_key,
        public_key,
        created_at: at,
        updated_at: at,
      }),
      _ => Err(BlogError::NotYetCreated),
    }
  }

  fn apply_next(state: Self::State, event: Self::Event) -> Result<Self::State, Self::Error> {
    todo!()
  }

  fn handle_first<'s, 'a: 's>(
    &'s self,
    id: &'a Self::Id,
    command: Self::Command,
  ) -> BoxFuture<'s, Result<Option<Vec<Self::Event>>, Self::Error>>
  where
    Self: Sized,
  {
    Err(BlogError::NotYetCreated)
  }

  fn handle_next<'a, 's: 'a>(
    &'a self,
    id: &'a Self::Id,
    state: &'s Self::State,
    command: Self::Command,
  ) -> BoxFuture<'a, Result<Option<Vec<Self::Event>>, Self::Error>>
  where
    Self: Sized,
  {
    todo!()
  }
}
