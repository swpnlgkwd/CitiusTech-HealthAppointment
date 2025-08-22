import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

interface ChatMessage {
  text: string;
  sender: 'user' | 'bot';
  time: string;
}

@Component({
  selector: 'app-chat-widget',
  templateUrl: './chat-widget.html',
  styleUrls: ['./chat-widget.css'],
  imports: [ CommonModule, ReactiveFormsModule],
  standalone: true
})
export class ChatWidgetComponent {
  isOpen = false;
  chatForm: FormGroup;
  messages: ChatMessage[] = [{text: "Welcome to MediMate! How can I assist you today?", sender: "bot", time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}];

  constructor(private fb: FormBuilder) {
    this.chatForm = this.fb.group({
      message: ['', Validators.required]
    });
  }

  toggleChat(): void {
    this.isOpen = !this.isOpen;
  }

  refreshChat(): void {
    this.messages = [];
  }

  private getCurrentTime(): string {
    return new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  sendMessage(): void {
    if (this.chatForm.invalid) return;

    const userMsg: ChatMessage = {
      text: this.chatForm.value.message,
      sender: 'user',
      time: this.getCurrentTime()
    };
    this.messages.push(userMsg);

    this.chatForm.reset();

    // Simulate bot reply after 1s
    setTimeout(() => {
      const botMsg: ChatMessage = {
        text: 'This is a smart response from MediMate 🤖',
        sender: 'bot',
        time: this.getCurrentTime()
      };
      this.messages.push(botMsg);
    }, 1000);
  }
}
