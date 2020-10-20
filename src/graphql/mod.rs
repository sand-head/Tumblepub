use juniper::RootNode;

pub struct QueryRoot;
#[juniper::object]
impl QueryRoot {

}

pub struct MutationRoot;
#[juniper::object]
impl MutationRoot {

}

pub type Schema = RootNode<'static, QueryRoot, MutationRoot>;

pub fn create_schema() -> Schema {
  Schema::new(QueryRoot {}, MutationRoot {})
}