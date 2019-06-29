import {combineReducers} from "redux";

import results from "./results_reducer";
import users from "./users_reducer";
import auth from "./auth_reducer";
import view from "./view_reducer";

export default combineReducers({
    results,
    users,
    auth,
    view
});
