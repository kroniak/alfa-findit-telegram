import React, {Component} from "react";
import Plate from "arui-feather/plate/";
import Heading from "arui-feather/heading/";
import Input from "arui-feather/input/";
import FormField from "arui-feather/form-field/";
import Form from "arui-feather/form/";
import Button from "arui-feather/button/";
import Notification from "../misc/notification";

class LoginForm extends Component {
    constructor(props) {
        super(props);

        this.state = {
            userField: "admin",
            passwordField: "CegthCtrhtnysqFlvbycrbqGfhjkm"
        };

        this.onSubmitForm = this.onSubmitForm.bind(this);
    }

    onChangeUserInputValue = value => {
        this.setState({
            userField: value
        });
    };

    onChangePasswordInputValue = value => {
        this.setState({
            passwordField: value
        });
    };

    onSubmitForm() {
        const {userField, passwordField} = this.state;
        if (userField && passwordField) {
            this.props.logUser(userField, passwordField);

            this.setState({
                    passwordField: "",
                }
            );
        }
    };

    render() {
        const {userField, passwordField} = this.state;
        return (
            <div style={{"marginBottom": "10px"}}>
                <Plate hasCloser={false}>
                    <Heading size='m'>
                        Пожалуйста войдите
                    </Heading>
                    <div>
                        <Notification
                            title='Ошибка входа'
                            message="Что то произошло не так! Скорее всего логин и пароль неверный"
                            error={this.props.error}>
                        </Notification>
                        <Form onSubmit={this.onSubmitForm}>
                            <FormField size='m'>
                                <Input
                                    size='m'
                                    placeholder='Логин'
                                    type='text'
                                    value={userField}
                                    onChange={this.onChangeUserInputValue}
                                />
                            </FormField>
                            <FormField size='m'>
                                <Input
                                    size='m'
                                    placeholder='Пароль'
                                    type='password'
                                    value={passwordField}
                                    onChange={this.onChangePasswordInputValue}
                                />
                            </FormField>
                            <FormField>
                                <Button view='extra' type='submit'>Войти</Button>
                            </FormField>
                        </Form>
                    </div>
                </Plate>
            </div>)
    }
}

export default LoginForm;