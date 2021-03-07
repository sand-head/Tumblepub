table! {
    blogs (id) {
        id -> Int8,
        uri -> Nullable<Varchar>,
        name -> Varchar,
        domain -> Nullable<Varchar>,
        is_public -> Bool,
        title -> Nullable<Varchar>,
        description -> Nullable<Text>,
        created_at -> Timestamp,
        updated_at -> Timestamp,
    }
}

table! {
    posts (id) {
        id -> Uuid,
        blog_id -> Int8,
    }
}

table! {
    user_blogs (user_id, blog_id) {
        user_id -> Int8,
        blog_id -> Int8,
        is_admin -> Bool,
    }
}

table! {
    users (id) {
        id -> Int8,
        email -> Varchar,
        primary_blog -> Int8,
        hash -> Bytea,
        salt -> Varchar,
        created_at -> Timestamp,
        updated_at -> Timestamp,
        last_login_at -> Timestamp,
    }
}

joinable!(posts -> blogs (blog_id));
joinable!(user_blogs -> blogs (blog_id));
joinable!(user_blogs -> users (user_id));
joinable!(users -> blogs (primary_blog));

allow_tables_to_appear_in_same_query!(
    blogs,
    posts,
    user_blogs,
    users,
);
