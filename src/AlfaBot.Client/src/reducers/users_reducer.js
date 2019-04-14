import * as actions from "../actions/types";

const initialState = {
    data: [],
    error: null,
    isLoading: false
};

export default (state = initialState, {type, payload}) => {
    switch (type) {
        case actions.USERS_FETCH_STARTED:
            return {
                ...state,
                isLoading: state.data.length === 0
            };

        case actions.USERS_FETCH_FAILED:
            return {
                ...state,
                error: payload,
                isLoading: false
            };

        case actions.USERS_FETCH_SUCCESS:
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
