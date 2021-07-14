﻿using System;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace Microsoft.Maui
{
	public static class EditTextExtensions
	{
		static readonly int[][] ColorStates =
		{
			new[] { Android.Resource.Attribute.StateEnabled },
			new[] { -Android.Resource.Attribute.StateEnabled }
		};

		public static void UpdateText(this AppCompatEditText editText, IEntry entry)
		{
			editText.UpdateText(entry.Text);

			// TODO ezhart The renderer sets the text to selected and shows the keyboard if the EditText is focused
		}

		public static void UpdateText(this AppCompatEditText editText, IEditor editor)
		{
			editText.UpdateText(editor.Text);

			editText.SetSelection(editText.Text?.Length ?? 0);
		}

		public static void UpdateTextColor(this AppCompatEditText editText, ITextStyle entry, ColorStateList? defaultColor)
		{
			var textColor = entry.TextColor;

			if (textColor == null)
			{
				if (defaultColor != null)
					editText.SetTextColor(defaultColor);
			}
			else
			{
				var androidColor = textColor.ToNative();

				if (!editText.TextColors.IsOneColor(ColorStates, androidColor))
				{
					var acolor = androidColor.ToArgb();
					editText.SetTextColor(new ColorStateList(ColorStates, new[] { acolor, acolor }));
				}
			}
		}

		public static void UpdateIsPassword(this AppCompatEditText editText, IEntry entry)
		{
			editText.SetInputType(entry);
		}

		public static void UpdateHorizontalTextAlignment(this AppCompatEditText editText, IEntry entry)
		{
			editText.UpdateHorizontalAlignment(entry.HorizontalTextAlignment, editText.Context != null && editText.Context.HasRtlSupport());
		}

		public static void UpdateVerticalTextAlignment(this AppCompatEditText editText, IEntry entry)
		{
			editText.UpdateVerticalAlignment(entry.VerticalTextAlignment);
		}

		public static void UpdateIsTextPredictionEnabled(this AppCompatEditText editText, IEntry entry)
		{
			editText.SetInputType(entry);
		}

		public static void UpdateIsTextPredictionEnabled(this AppCompatEditText editText, IEditor editor)
		{
			if (editor.IsTextPredictionEnabled)
				editText.InputType &= ~InputTypes.TextFlagNoSuggestions;
			else
				editText.InputType |= InputTypes.TextFlagNoSuggestions;
		}

		public static void UpdateMaxLength(this AppCompatEditText editText, IEntry entry) =>
			UpdateMaxLength(editText, entry.MaxLength);

		public static void UpdateMaxLength(this AppCompatEditText editText, IEditor editor) =>
			UpdateMaxLength(editText, editor.MaxLength);

		public static void UpdateMaxLength(this AppCompatEditText editText, int maxLength)
		{
			editText.SetLengthFilter(maxLength);

			var newText = editText.Text.TrimToMaxLength(maxLength);
			if (editText.Text != newText)
				editText.Text = newText;
		}

		public static void SetLengthFilter(this EditText editText, int maxLength)
		{
			var currentFilters = new List<IInputFilter>(editText.GetFilters() ?? new IInputFilter[0]);
			var changed = false;

			for (var i = 0; i < currentFilters.Count; i++)
			{
				if (currentFilters[i] is InputFilterLengthFilter)
				{
					currentFilters.RemoveAt(i);
					changed = true;
					break;
				}
			}

			if (maxLength > 0)
			{
				currentFilters.Add(new InputFilterLengthFilter(maxLength));
				changed = true;
			}

			if (changed)
				editText.SetFilters(currentFilters.ToArray());
		}

		public static void UpdatePlaceholder(this AppCompatEditText editText, IPlaceholder textInput)
		{
			if (editText.Hint == textInput.Placeholder)
				return;

			editText.Hint = textInput.Placeholder;
		}

		public static void UpdatePlaceholderColor(this AppCompatEditText editText, IEditor editor, ColorStateList? defaultColor)
		{
			var placeholderTextColor = editor.PlaceholderColor;
			if (placeholderTextColor == null)
			{
				editText.SetHintTextColor(defaultColor);
			}
			else
			{
				var androidColor = placeholderTextColor.ToNative();

				if (!editText.HintTextColors.IsOneColor(ColorExtensions.States, androidColor))
				{
					var acolor = androidColor.ToArgb();
					editText.SetHintTextColor(new ColorStateList(ColorExtensions.States, new[] { acolor, acolor }));
				}
			}
		}

		public static void UpdateIsReadOnly(this AppCompatEditText editText, IEntry entry)
		{
			bool isEditable = !entry.IsReadOnly;

			editText.SetInputType(entry);

			editText.FocusableInTouchMode = isEditable;
			editText.Focusable = isEditable;
		}

		public static void UpdateKeyboard(this AppCompatEditText editText, IEntry entry)
		{
			editText.SetInputType(entry);
		}

		public static void UpdateKeyboard(this AppCompatEditText editText, IEditor editor)
		{
			editText.SetInputType(editor);
		}

		public static void UpdateIsReadOnly(this AppCompatEditText editText, IEditor editor)
		{
			bool isReadOnly = !editor.IsReadOnly;

			editText.FocusableInTouchMode = isReadOnly;
			editText.Focusable = isReadOnly;
			editText.SetCursorVisible(isReadOnly);
		}

		public static void UpdateClearButtonVisibility(this AppCompatEditText editText, IEntry entry, Drawable? clearButtonDrawable) =>
			UpdateClearButtonVisibility(editText, entry, () => clearButtonDrawable);

		public static void UpdateClearButtonVisibility(this AppCompatEditText editText, IEntry entry, Func<Drawable?>? getClearButtonDrawable)
		{
			// Places clear button drawable at the end or start of the EditText based on FlowDirection.
			void ShowClearButton()
			{
				var drawable = getClearButtonDrawable?.Invoke();

				if (entry.FlowDirection == FlowDirection.RightToLeft)
				{
					editText.SetCompoundDrawablesWithIntrinsicBounds(drawable, null, null, null);
				}
				else
				{
					editText.SetCompoundDrawablesWithIntrinsicBounds(null, null, drawable, null);
				}
			}

			// Hides clear button drawable from the control.
			void HideClearButton()
			{
				editText.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
			}

			bool isFocused = editText.IsFocused;
			bool hasText = entry.Text?.Length > 0;

			bool shouldDisplayClearButton = entry.ClearButtonVisibility == ClearButtonVisibility.WhileEditing
				&& hasText
				&& isFocused;

			if (shouldDisplayClearButton)
			{
				ShowClearButton();
			}
			else
			{
				HideClearButton();
			}
		}

		public static void UpdateReturnType(this AppCompatEditText editText, IEntry entry)
		{
			editText.ImeOptions = entry.ReturnType.ToNative();
		}

		[PortHandler]
		public static void UpdateCursorPosition(this AppCompatEditText editText, IEntry entry)
		{
			if (editText.SelectionStart != entry.CursorPosition)
				UpdateCursorSelection(editText, entry);
		}

		[PortHandler]
		public static void UpdateSelectionLength(this AppCompatEditText editText, IEntry entry)
		{
			if ((editText.SelectionEnd - editText.SelectionStart) != entry.SelectionLength)
				UpdateCursorSelection(editText, entry);
		}

		/* Updates both the IEntry.CursorPosition and IEntry.SelectionLength properties. */
		static void UpdateCursorSelection(AppCompatEditText editText, IEntry entry)
		{
			if (!entry.IsReadOnly)// && editText.HasFocus)// || editText.RequestFocus()))//&& editText.RequestFocus())
			{
				if (!editText.HasFocus)
					editText.RequestFocus();

				int start = GetSelectionStart(editText, entry);
				int end = GetSelectionEnd(editText, entry, start);

				editText.SetSelection(start, end);
			}
		}

		static int GetSelectionStart(AppCompatEditText editText, IEntry entry)
		{
			int start = editText.Length();
			int cursorPosition = entry.CursorPosition;

			if (editText.Text != null)
			{
				// Capping cursorPosition to the end of the text if needed
				start = System.Math.Min(editText.Text.Length, cursorPosition);
			}

			if (start != cursorPosition)
			{
				// Update the interface if start was capped
				entry.CursorPosition = start;
			}

			return start;
		}

		static int GetSelectionEnd(AppCompatEditText editText, IEntry entry, int start)
		{
			int end = start;
			int selectionLength = entry.SelectionLength;
			end = System.Math.Max(start, System.Math.Min(editText.Length(), start + selectionLength));
			int newSelectionLength = System.Math.Max(0, end - start);
			// Updating this property results in UpdateSelectionLength being called again messing things up
			if (newSelectionLength != selectionLength)
				entry.SelectionLength = newSelectionLength;
			return end;
		}

		internal static void SetInputType(this AppCompatEditText editText, IEditor editor)
		{
			if (editor.IsReadOnly)
			{
				editText.InputType = InputTypes.Null;
			}
			else
			{
				var keyboard = editor.Keyboard;
				var nativeInputTypeToUpdate = keyboard.ToInputType();

				if (keyboard is not CustomKeyboard)
				{
					// TODO: IsSpellCheckEnabled handling must be here.

					if ((nativeInputTypeToUpdate & InputTypes.TextFlagNoSuggestions) != InputTypes.TextFlagNoSuggestions)
					{
						if (!editor.IsTextPredictionEnabled)
							nativeInputTypeToUpdate |= InputTypes.TextFlagNoSuggestions;
					}
				}

				if (keyboard == Keyboard.Numeric)
				{
					editText.KeyListener = LocalizedDigitsKeyListener.Create(editText.InputType);
				}

				editText.InputType = nativeInputTypeToUpdate;
			}
		}

		internal static void SetInputType(this AppCompatEditText editText, IEntry entry)
		{
			if (entry.IsReadOnly)
			{
				editText.InputType = InputTypes.Null;
			}
			else
			{
				var keyboard = entry.Keyboard;
				var nativeInputTypeToUpdate = keyboard.ToInputType();

				if (keyboard is not CustomKeyboard)
				{
					// TODO: IsSpellCheckEnabled handling must be here.

					if ((nativeInputTypeToUpdate & InputTypes.TextFlagNoSuggestions) != InputTypes.TextFlagNoSuggestions)
					{
						if (!entry.IsTextPredictionEnabled)
							nativeInputTypeToUpdate |= InputTypes.TextFlagNoSuggestions;
					}
				}

				if (keyboard == Keyboard.Numeric)
				{
					editText.KeyListener = LocalizedDigitsKeyListener.Create(editText.InputType);
				}

				if (entry.IsPassword)
				{
					if ((nativeInputTypeToUpdate & InputTypes.ClassText) == InputTypes.ClassText)
						nativeInputTypeToUpdate |= InputTypes.TextVariationPassword;

					if ((nativeInputTypeToUpdate & InputTypes.ClassNumber) == InputTypes.ClassNumber)
						nativeInputTypeToUpdate |= InputTypes.NumberVariationPassword;
				}

				editText.InputType = nativeInputTypeToUpdate;
			}
		}
	}
}
