CREATE TABLE user_data (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(255) NOT NULL UNIQUE,
    password TEXT NOT NULL
);

CREATE TABLE savegames (
    savegame_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    data JSONB DEFAULT '{}'::jsonb,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES user_data(user_id) ON DELETE CASCADE
);
