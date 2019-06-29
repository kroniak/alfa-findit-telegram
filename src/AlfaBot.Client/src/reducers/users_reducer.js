import {USERS_FETCH_FAILED, USERS_FETCH_STARTED, USERS_FETCH_SUCCESS} from "../actions/types";

const initialState = {
    data: [],
    error: null,
    isLoading: false
};

export default (state = initialState, {type, payload}) => {
    switch (type) {
        case USERS_FETCH_STARTED:
            return {
                ...state,
                isLoading: true,
                error: null
            };

        case USERS_FETCH_FAILED:
            return {
                ...state,
                error: payload,
                isLoading: false
            };

        case USERS_FETCH_SUCCESS:
            return {
                ...state,
                data: payload,
                error: null,
                isLoading: false
            };

        default:
            return state;
    }
};
